using System.Data;
using Dapper;
using OrderManagementApi.DTOs;
using OrderManagementApi.Models;

namespace OrderManagementApi.Data.Repository
{
    public class OrderStatusRepository : BaseRepository
    {
        #region Constructor

        public OrderStatusRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        #endregion

        #region Methods

        public async Task<List<OrderStatusDto>> GetAllStatusesAsync()
        {
            var sql = @"
                SELECT id, description
                FROM [dbo].[order_status]
                ORDER BY id;
            ";

            var statusModels = await _dbConnection.QueryAsync<OrderStatus>(sql);

            var statusDtos = statusModels
                .Select(s => new OrderStatusDto
                {
                    Id = s.Id,
                    Description = s.Description
                })
                .ToList();

            return statusDtos;
        }

        public async Task<string> GetStatusDescriptionByIdAsync(short statusId, IDbTransaction? transaction = null)
        {
            string sql = @"
                SELECT description
                FROM [dbo].[order_status]
                WHERE id = @StatusId;
            ";

            var description = await _dbConnection.ExecuteScalarAsync<string>(sql, new { StatusId = statusId }, transaction);

            if (string.IsNullOrWhiteSpace(description))
                return $"Status com ID {statusId} não encontrado.";

            return description;
        }

        #endregion
    }
}
