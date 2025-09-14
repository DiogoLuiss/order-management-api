using System.Data;
using Dapper;
using OrderManagementApi.Data.Dapper;
using OrderManagementApi.DTOs;
using OrderManagementApi.Exceptions;
using OrderManagementApi.Models;

namespace OrderManagementApi.Data.Repository
{
    public class ProductRepository : BaseRepository
    {
        #region Atributs

        private readonly IDapperWrapper _dapper;

        #endregion

        #region Constructor

        public ProductRepository(
            IDbConnection dbConnection,
            IDapperWrapper dapperWrapper) : base(dbConnection)
        {
            _dapper = dapperWrapper;
        }

        #endregion

        #region Methods

        public async Task CreateProductAsync(string name, string description, decimal price, int stockQuantity)
        {
            string sql = @"
                INSERT INTO product (name, description, price, stock_quantity)
                VALUES (@Name, @Description, @Price, @StockQuantity);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var parameters = new
            {
                Name = name,
                Description = description,
                Price = price,
                StockQuantity = stockQuantity
            };

            await _dbConnection.ExecuteAsync(sql, parameters);
        }

        public async Task<ProductListResponseDto> GetProductListAsync(string? nameOrDescription, int page = 1)
        {
            const int pageSize = 50;

            string sqlBase = @"
            SELECT id, name, description, price, stock_quantity
            FROM product";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(nameOrDescription))
            {
                sqlBase += " WHERE name LIKE @search OR description LIKE @search";
                parameters.Add("search", $"{nameOrDescription}%");
            }

            string countSql = $"SELECT COUNT(*) FROM ({sqlBase}) AS countQuery";
            int totalItems = await _dbConnection.ExecuteScalarAsync<int>(countSql, parameters);

            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            sqlBase += " ORDER BY id OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            parameters.Add("offset", (page - 1) * pageSize);
            parameters.Add("pageSize", pageSize);

            var products = await _dbConnection.QueryAsync<Product>(sqlBase, parameters);

            return new ProductListResponseDto
            {
                Products = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<bool> UpdateProductAsync(int id, string name, string description, decimal price, int stockQuantity)
        {
            string sql = @"
                UPDATE product
                SET name = @Name,
                    description = @Description,
                    price = @Price,
                    stock_quantity = @StockQuantity
                WHERE id = @Id";

            var parameters = new
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price,
                StockQuantity = stockQuantity
            };

            int affectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var count = await this.CountOrdersWithProductAsyncInternal(id);

            if (count > 0) 
                throw new BadRequestException($"Existem {count} pedido(s) criado(s) para esse produto, não será possível excluir.");

            string sql = "DELETE FROM product WHERE id = @Id";
            var parameters = new { Id = id };
            int affectedRows = await _dapper.ExecuteAsync(sql, parameters);
            return affectedRows > 0;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            string sql = @"
            SELECT 
                id,
                name,
                description,
                price,
                stock_quantity
            FROM product
            WHERE id = @Id";

            var parameters = new { Id = id };

            var product = await _dbConnection.QuerySingleOrDefaultAsync<Product>(sql, parameters);

            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };
        }

        public async Task<decimal> GetProductPriceAsync(int productId, IDbTransaction? transaction = null)
        {
            string sql = @"
                SELECT price
                FROM product
                WHERE id = @Id
            ";

            var price = await _dbConnection.QuerySingleOrDefaultAsync<decimal>(sql, new { Id = productId }, transaction);
            return price;
        }

        public async Task<ProductStockDto> CheckProductExistsAndStockAsync(int productId)
        {
            string sql = @"
                SELECT name, stock_quantity
                FROM product
                WHERE id = @Id
            ";

            var product = await _dbConnection.QuerySingleOrDefaultAsync<(string Name, int StockQuantity)>(sql, new { Id = productId });

            if (product == default)
                return new ProductStockDto { Exists = false };

            return new ProductStockDto
            {
                Exists = true,
                Name = product.Name,
                StockQuantity = product.StockQuantity
            };
        }

        public async Task DecreaseStockAsync(int productId, int quantity, IDbTransaction transaction)
        {
            const string sql = @"
                UPDATE product
                SET stock_quantity = stock_quantity - @Quantity
                WHERE id = @ProductId
                  AND stock_quantity >= @Quantity;
            ";

            int affectedRows = await _dbConnection.ExecuteAsync(
                sql,
                new { ProductId = productId, Quantity = quantity },
                transaction
            );

            if (affectedRows == 0)
                throw new InvalidOperationException($"Não foi possível decrementar o estoque do produto {productId}. Estoque insuficiente.");
        }

        public async Task<ProductSimpleListResponseDto> GetProductListSimpleAsync(string? nameOrDescription, int page = 1)
        {
            const int pageSize = 50;
            var parameters = new DynamicParameters();
            string whereClause = "";

            if (!string.IsNullOrWhiteSpace(nameOrDescription))
            {
                whereClause = " WHERE name LIKE @search OR description LIKE @search";
                parameters.Add("search", $"{nameOrDescription}%");
            }

            string sqlCount = $"SELECT COUNT(*) FROM product{whereClause}";
            int totalProducts = await _dbConnection.ExecuteScalarAsync<int>(sqlCount, parameters);

            int totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            int offset = (page - 1) * pageSize;

            string sql = $@"
            SELECT 
                id,
                name,
                description,
                price
            FROM product
            {whereClause}
            ORDER BY name
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var products = await _dbConnection.QueryAsync<Product>(sql, parameters);

            return new ProductSimpleListResponseDto
            {
                Products = products.Select(p => new ProductSimpleDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        #endregion

        #region 

        #region Private Methods 

        private async Task<int> CountOrdersWithProductAsyncInternal(int productId)
        {
            var sql = @"
                SELECT COUNT(DISTINCT order_id)
                FROM [dbo].[item_order]
                WHERE product_id = @ProductId;
            ";

            return await _dapper.ExecuteScalarAsync<int>(sql, new { ProductId = productId });
        }

        #endregion


        #endregion
    }
}
