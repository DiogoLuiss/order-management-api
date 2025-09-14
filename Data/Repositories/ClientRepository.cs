using Dapper;
using OrderManagementApi.Data.Dapper;
using OrderManagementApi.DTOs;
using OrderManagementApi.Exceptions;
using OrderManagementApi.Models;
using System.Data;

namespace OrderManagementApi.Data.Repository
{
    public class ClientRepository : BaseRepository
    {
        #region Atributs

        private readonly IDapperWrapper _dapper;

        #endregion

        #region Constructor

        public ClientRepository(IDbConnection dbConnection, IDapperWrapper dapper) : base(dbConnection)
        {
            _dapper = dapper;
        }

        #endregion

        #region Methods

        public async Task<bool> ExistsAsync(int clientId)
        {
            const string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM client WHERE id = @Id) THEN 1 ELSE 0 END";
            return await _dbConnection.ExecuteScalarAsync<bool>(sql, new { Id = clientId });
        }

        public async Task<ClientDto?> GetClientByIdAsync(int id)
        {
            string sql = @"
                SELECT 
                    id,
                    name,
                    email,
                    phone,
                    created_at
                FROM client
                WHERE id = @Id";

            var client = await _dbConnection.QueryFirstOrDefaultAsync<Client>(sql, new { Id = id });

            if (client == null)
                return null;

            return new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<ClientListResponseDto> GetClientListAsync(string? nameOrEmail, int page = 1)
        {
            const int pageSize = 50;
            var parameters = new DynamicParameters();
            string whereClause = "";

            if (!string.IsNullOrWhiteSpace(nameOrEmail))
            {
                whereClause = " WHERE name LIKE @search OR email LIKE @search";
                parameters.Add("search", $"{nameOrEmail}%");
            }

            string sqlCount = $"SELECT COUNT(*) FROM client{whereClause}";
            int totalClients = await _dbConnection.ExecuteScalarAsync<int>(sqlCount, parameters);

            int totalPages = (int)Math.Ceiling(totalClients / (double)pageSize);
            int offset = (page - 1) * pageSize;

            string sql = $@"
            SELECT 
                id,
                name,
                email,
                phone,
                created_at
            FROM client
            {whereClause}
            ORDER BY created_at DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var clients = await _dbConnection.QueryAsync<Client>(sql, parameters);

            return new ClientListResponseDto
            {
                Clients = clients.Select(c => new ClientDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task CreateClientAsync(string name, string email, string phone)
        {
            string checkSql = @"
            SELECT COUNT(1)
            FROM client
            WHERE name = @Name OR email = @Email";

            var exists = await _dbConnection.ExecuteScalarAsync<int>(checkSql, new { Name = name, Email = email });

            if (exists > 0)
                throw new BadRequestException("Já existe um cliente com este nome ou e-mail.");

            string sql = @"
                INSERT INTO client (name, email, phone)
                VALUES (@Name, @Email, @Phone)";

            var parameters = new DynamicParameters();
            parameters.Add("Name", name);
            parameters.Add("Email", email);
            parameters.Add("Phone", phone);

            await _dbConnection.ExecuteAsync(sql, parameters);
        }

        public async Task<bool> UpdateClientAsync(int id, string? name, string? email, string? phone)
        {
            var setClauses = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            if (name != null)
            {
                setClauses.Add("name = @Name");
                parameters.Add("Name", name);
            }

            if (email != null)
            {
                setClauses.Add("email = @Email");
                parameters.Add("Email", email);
            }

            if (phone != null)
            {
                setClauses.Add("phone = @Phone");
                parameters.Add("Phone", phone);
            }

            if (!setClauses.Any())
                return true;

            string sql = $"UPDATE client SET {string.Join(", ", setClauses)} WHERE id = @Id";

            int affectedRows = await _dbConnection.ExecuteAsync(sql, parameters);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            int count = await this.CountOrdersByClientAsync(id);

            if (count > 0)
                throw new BadRequestException($"Existem {count} pedido(s) criado(s) para este cliente, não será possível excluir.");

            string sql = "DELETE FROM client WHERE id = @Id";

            int affectedRows = await _dapper.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<ClientSimpleListResponseDto> GetClientListSimpleAsync(string? nameOrEmail, int page = 1)
        {
            const int pageSize = 50;
            var parameters = new DynamicParameters();
            string whereClause = "";

            if (!string.IsNullOrWhiteSpace(nameOrEmail))
            {
                whereClause = " WHERE name LIKE @search OR email LIKE @search";
                parameters.Add("search", $"{nameOrEmail}%");
            }

            string sqlCount = $"SELECT COUNT(*) FROM client{whereClause}";
            int totalClients = await _dbConnection.ExecuteScalarAsync<int>(sqlCount, parameters);

            int totalPages = (int)Math.Ceiling(totalClients / (double)pageSize);
            int offset = (page - 1) * pageSize;

            string sql = $@"
                SELECT 
                    id,
                    name,
                    email,
                    phone
                FROM client
                {whereClause}
                ORDER BY name
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            parameters.Add("Offset", offset);
            parameters.Add("PageSize", pageSize);

            var clients = await _dbConnection.QueryAsync<Client>(sql, parameters);

            return new ClientSimpleListResponseDto
            {
                Clients = clients.Select(c => new ClientSimpleDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Email = c.Email,
                    Phone = c.Phone
                }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        #endregion

        #region Private Methods

        public async Task<int> CountOrdersByClientAsync(int clientId)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM [dbo].[order]
                WHERE client_id = @ClientId;
            ";

            return await _dapper.ExecuteScalarAsync<int>(sql, new { ClientId = clientId });
        }

        #endregion
    }
}
