using System.Data;
using Dapper;
using OrderManagementApi.DTOs;
using OrderManagementApi.Models;

namespace OrderManagementApi.Data.Repository
{
    public class ProductRepository : BaseRepository
    {
        #region Constructor

        public ProductRepository(IDbConnection dbConnection) : base(dbConnection)
        {
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

        public async Task<List<ProductDto>> GetProductListAsync(string? nameOrDescription)
        {
            string sql = @"
                SELECT 
                    id,
                    name,
                    description,
                    price,
                    stock_quantity
                FROM product";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(nameOrDescription))
            {
                sql += " WHERE name LIKE @search OR description LIKE @search";
                parameters.Add("search", $"{nameOrDescription}%");
            }

            var products = await _dbConnection.QueryAsync<Product>(sql, parameters);

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }).ToList();
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
            string sql = "DELETE FROM product WHERE id = @Id";
            var parameters = new { Id = id };
            int affectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
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

        public async Task<List<SimpleProductDto>> GetSimpleProductListAsync(string? nameOrDescription)
        {
            string sql = @"
                SELECT 
                    id,
                    name,
                    description,
                    price
                FROM product";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(nameOrDescription))
            {
                sql += " WHERE name LIKE @search OR description LIKE @search";
                parameters.Add("search", $"{nameOrDescription}%");
            }

            var products = await _dbConnection.QueryAsync<Product>(sql, parameters);

            return products.Select(p => new SimpleProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            }).ToList();
        }


        #endregion
    }
}
