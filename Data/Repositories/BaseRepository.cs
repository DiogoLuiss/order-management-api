using System.Data;

namespace OrderManagementApi.Data.Repository
{
    public abstract class BaseRepository
    {
        #region Propriedades

        protected readonly IDbConnection _dbConnection;

        #endregion

        #region Construtor

        protected BaseRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #endregion
    }
}
