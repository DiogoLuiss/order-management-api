using System.Data;

namespace OrderManagementApi.Data.Repository
{
    public abstract class BaseRepository
    {
        #region Properties

        protected readonly IDbConnection _dbConnection;

        #endregion

        #region Constructor

        protected BaseRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        #endregion
    }
}
