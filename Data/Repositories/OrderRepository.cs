using Dapper;
using OrderManagementApi.DTOs;
using OrderManagementApi.Exceptions;
using OrderManagementApi.Models;
using System.Data;

namespace OrderManagementApi.Data.Repository
{
    public class OrderRepository : BaseRepository
    {
        #region Atributs

        private readonly ProductRepository _productRepository;
        private readonly ClientRepository _clientRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly OrderStatusRepository _orderStatusRepository;

        #endregion

        #region Constructor

        public OrderRepository(
            IDbConnection dbConnection,
            ProductRepository productRepository,
            ClientRepository clientRepository,
            NotificationRepository notificationRepository,
            OrderStatusRepository orderStatusRepository) : base(dbConnection)
        {
            _productRepository = productRepository;
            _clientRepository = clientRepository;
            _notificationRepository = notificationRepository;
            _orderStatusRepository = orderStatusRepository; 
        }

        #endregion

        #region Methods

        public async Task UpdateOrderStatusAsync(int orderId, short newStatusId)
        {
            #region Validations

            string checkOrderSql = @"
                SELECT COUNT(1)
                FROM [dbo].[order]
                WHERE id = @OrderId;
            ";

            int orderExists = await _dbConnection.ExecuteScalarAsync<int>(checkOrderSql, new { OrderId = orderId });

            if (orderExists == 0)
                throw new BadRequestException($"Pedido com ID {orderId} não encontrado.");

            string checkStatusSql = @"
                SELECT COUNT(1)
                FROM [dbo].[order_status]
                WHERE id = @StatusId;
            ";

            int statusExists = await _dbConnection.ExecuteScalarAsync<int>(checkStatusSql, new { StatusId = newStatusId });

            if (statusExists == 0)
                throw new BadRequestException($"Status com ID {newStatusId} não encontrado.");

            #endregion

            var oldStatus = await GetOrderStatusDescriptionAsync(orderId);

            string sql = @"
                UPDATE [dbo].[order]
                SET status_id = @NewStatusId
                WHERE id = @OrderId;
            ";

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    int affectedRows = await _dbConnection.ExecuteAsync(sql, new
                    {
                        OrderId = orderId,
                        NewStatusId = newStatusId
                    }, transaction);

                    if (affectedRows == 0)
                        throw new BadRequestException($"Pedido com ID {orderId} não encontrado ou status não alterado.");

                    string newStatusDescription = await _orderStatusRepository.GetStatusDescriptionByIdAsync(newStatusId, transaction);
                    string message = $"Pedido #{orderId} mudou de '{oldStatus}' para '{newStatusDescription}'.";
                    await _notificationRepository.CreateNotificationAsync(orderId, message, transaction);

                    transaction.Commit();
                }
                catch (BadRequestException ex)
                {
                    transaction.Rollback();
                    throw new BadRequestException(ex.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<OrderStatusDto> GetOrderStatusByIdAsync(int orderId)
        {
            string sql = @"
                SELECT s.id, s.description
                FROM [dbo].[order] o
                INNER JOIN [dbo].[order_status] s ON s.id = o.status_id
                WHERE o.id = @OrderId;
            ";

            var status = await _dbConnection.QuerySingleOrDefaultAsync<OrderStatusDto>(
                sql, new { OrderId = orderId }
            );

            if (status == null)
                throw new KeyNotFoundException($"Pedido com ID {orderId} não encontrado.");

            return status;
        }

        public async Task<int> CreateOrderAsync(CreateOrderDto dto)
        {
            #region Validations

            bool clientExists = await _clientRepository.ExistsAsync(dto.ClientId);

            if (!clientExists)
                throw new BadRequestException($"Cliente passado não foi encontrado.");

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.CheckProductExistsAndStockAsync(item.ProductId);

                if (!product.Exists)
                    throw new BadRequestException($"Produto não encontrado.");

                if (product.StockQuantity < item.Quantity)
                    throw new BadRequestException($"Produto '{product.Name}' não possui estoque suficiente. Estoque atual: {product.StockQuantity}, solicitado: {item.Quantity}.");
            }

            #endregion

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    #region Create Order

                    var sqlOrder = @"
                        INSERT INTO [dbo].[order] (client_id, total_value)
                        VALUES (@ClientId, @TotalValue);
                        SELECT CAST(SCOPE_IDENTITY() as int);
                        ";

                    var orderId = await _dbConnection.ExecuteScalarAsync<int>(sqlOrder, new
                    {
                        dto.ClientId,
                        dto.TotalValue,
                    },
                    transaction);

                    #endregion

                    #region Insert Order Items

                    decimal totalValue = 0;

                    if (dto.Items != null && dto.Items.Any())
                    {
                        var sqlItems = @"
                        INSERT INTO [dbo].[item_order] (order_id, product_id, quantity, unit_price)
                        VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
                    ";

                        foreach (var item in dto.Items)
                        {
                            var unitPrice = await _productRepository.GetProductPriceAsync(item.ProductId, transaction);
                            totalValue += unitPrice * item.Quantity;

                            await _dbConnection.ExecuteAsync(sqlItems, new
                            {
                                OrderId = orderId,
                                item.ProductId,
                                item.Quantity,
                                UnitPrice = unitPrice
                            },
                            transaction);

                            await _productRepository.DecreaseStockAsync(item.ProductId, item.Quantity, transaction);
                        }
                    }

                    #endregion

                    #region Update Order Total

                    var sqlUpdateTotal = @"
                        UPDATE [dbo].[order]
                        SET total_value = @TotalValue
                        WHERE id = @OrderId
                    ";

                    await _dbConnection.ExecuteAsync(
                        sqlUpdateTotal,
                        new { TotalValue = totalValue, OrderId = orderId },
                        transaction
                    );

                    #endregion

                    transaction.Commit();
                    return orderId;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync(string? clientName = null, short? orderStatusId = null)
        {
            var sql = @"
                SELECT 
                    o.id, o.order_date, o.total_value, o.status_id,
                    c.id, c.name,
                    s.id, s.description
                FROM [dbo].[order] o
                INNER JOIN [dbo].[client] c ON c.id = o.client_id
                INNER JOIN [dbo].[order_status] s ON s.id = o.status_id
                WHERE (@ClientName IS NULL OR c.name LIKE @ClientName)
                  AND (@OrderStatusId IS NULL OR o.status_id = @OrderStatusId)
                ORDER BY o.order_date DESC;
            ";

            var orderDictionary = new Dictionary<int, Order>();

            var orders = await _dbConnection.QueryAsync<Order, Client, OrderStatus, Order>(
                sql,
                (order, client, status) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Client = client;
                        orderEntry.Status = status;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }
                    return orderEntry;
                },
                new
                {
                    ClientName = string.IsNullOrWhiteSpace(clientName) ? null : $"{clientName}%",
                    OrderStatusId = orderStatusId
                }
            );

            var orderDtos = orders.Distinct().Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalValue = o.TotalValue,
                NomeClinete = o.Client?.Name ?? string.Empty,
                Status = o.Status?.Description ?? string.Empty
            });

            return orderDtos.ToList();
        }

        public async Task<OrderDetailsDto> GetOrderByIdAsync(int orderId)
        {
            var sqlOrder = @"
                SELECT 
                    o.id, o.order_date, o.total_value, o.status_id,
                    c.id, c.name,
                    s.id, s.description
                FROM [dbo].[order] o
                INNER JOIN [dbo].[client] c ON c.id = o.client_id
                INNER JOIN [dbo].[order_status] s ON s.id = o.status_id
                WHERE o.id = @OrderId;
            ";

                    var sqlItems = @"
                SELECT 
                    i.quantity,
                    p.name,
                    p.description
                FROM [dbo].[item_order] i
                INNER JOIN [dbo].[product] p ON p.id = i.product_id
                WHERE i.order_id = @OrderId;
            ";

            var orderDictionary = new Dictionary<int, Order>();
            var orders = await _dbConnection.QueryAsync<Order, Client, OrderStatus, Order>(
                sqlOrder,
                (order, client, status) =>
                {
                    if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                    {
                        orderEntry = order;
                        orderEntry.Client = client;
                        orderEntry.Status = status;
                        orderDictionary.Add(orderEntry.Id, orderEntry);
                    }
                    return orderEntry;
                },
                new { OrderId = orderId }
            );

            var order = orders.Distinct().FirstOrDefault();

            if (order == null)
                throw new KeyNotFoundException($"Pedido com ID {orderId} não encontrado.");

            var items = await _dbConnection.QueryAsync<OrderItemDetailsDto>(
                sqlItems,
                new { OrderId = orderId }
            );

            return new OrderDetailsDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalValue = order.TotalValue,
                NomeCliente = order.Client?.Name ?? string.Empty,
                Status = order.Status?.Description ?? string.Empty,
                Items = items.ToList()
            };
        }

        #endregion

        #region Private Methods

        private async Task<string> GetOrderStatusDescriptionAsync(int orderId)
        {
            string sql = @"
                SELECT s.description 
                FROM [dbo].[order] o
                INNER JOIN [dbo].[order_status] s ON s.id = o.status_id
                WHERE o.id = @OrderId;
            ";

            var statusDescription = await _dbConnection.ExecuteScalarAsync<string>(sql, new { OrderId = orderId });

            if (string.IsNullOrWhiteSpace(statusDescription))
                throw new KeyNotFoundException($"Status do pedido com ID {orderId} não encontrado.");

            return statusDescription;
        }

        #endregion
    }
}
