using Dapper;
using OrderManagementApi.Models;
using System.Data;

namespace OrderManagementApi.Data.Repository
{
    public class NotificationRepository : BaseRepository
    {
        #region Constructor

        public NotificationRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        #endregion

        #region Methods

        public async Task<List<Notification>> GetNotificationsByOrderIdAsync(int orderId)
        {
            string sql = @"
                SELECT 
                    id,
                    order_id AS OrderId,
                    message AS Message,
                    created_at AS CreatedAt
                FROM notification
                WHERE order_id = @OrderId
                ORDER BY created_at DESC";

            var notifications = await _dbConnection.QueryAsync<Notification>(sql, new { OrderId = orderId });
            return notifications.ToList();
        }

        public async Task CreateNotificationAsync(int orderId, string message, IDbTransaction? transaction = null)
        {
            string sql = @"
                INSERT INTO notification (order_id, message)
                VALUES (@OrderId, @Message)";

            var parameters = new DynamicParameters();
            parameters.Add("OrderId", orderId);
            parameters.Add("Message", message);

            await _dbConnection.ExecuteAsync(sql, parameters, transaction);
        }

        public async Task<List<Notification>> GetAllNotificationsAsync()
        {
            string sql = @"
                SELECT 
                    id,
                    order_id AS OrderId,
                    message AS Message,
                    created_at AS CreatedAt
                FROM notification
                ORDER BY created_at DESC";

            var notifications = await _dbConnection.QueryAsync<Notification>(sql);
            return notifications.ToList();
        }

        #endregion
    }
}
