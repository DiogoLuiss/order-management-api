using System.Data;
using Dapper;

namespace OrderManagementApi.Data.Dapper
{
    public class DapperWrapper : IDapperWrapper
    {
        private readonly IDbConnection _dbConnection;

        public DapperWrapper(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null)
        {
            return _dbConnection.ExecuteScalarAsync<T>(sql, param, transaction);
        }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null)
        {
            return _dbConnection.ExecuteAsync(sql, param, transaction);
        }
    }
}
