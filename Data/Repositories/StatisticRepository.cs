using System.Data;
using Dapper;
using OrderManagementApi.Data.Repository;
using OrderManagementApi.DTOs;

namespace OrderManagementApi.Data.Repositories
{
    public class StatisticRepository : BaseRepository
    {
        #region Constructor

        public StatisticRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        #endregion

        #region Methods
        public async Task<CountsDto> GetCountsAsync()
        {
            string sql = @"
                SELECT
                    (SELECT COUNT(*) FROM client) AS CountClients,
                    (SELECT COUNT(*) FROM product) AS CountProducts,
                    (SELECT COUNT(*) FROM [order]) AS CountOrders;
            ";

            var counts = await _dbConnection.QueryFirstAsync<CountsDto>(sql);
            return counts;
        }

        #endregion
    }
}
