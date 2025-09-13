using System.Data;
using Dapper;
using OrderManagementApi.DTOs;
using OrderManagementApi.Models;

namespace OrderManagementApi.Data.Repository
{
    public class OrderStatusRepository : BaseRepository
    {
        public OrderStatusRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

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
    }
}
