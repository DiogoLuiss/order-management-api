using System.Data;

namespace OrderManagementApi.Data.Dapper
{
    public interface IDapperWrapper
    {
        Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);
    }
}
