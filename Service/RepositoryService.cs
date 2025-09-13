using OrderManagementApi.Data.Repository;
using System.Reflection;

namespace OrderManagementApi.Service
{
    public static class RepositoryService
    {
        public static void AddRepositories(this IServiceCollection services, Assembly assembly)
        {
            #region BaseRepository

            var baseRepositoryType = typeof(BaseRepository);

            var repositoryTypes = assembly.GetTypes()
                .Where(t => baseRepositoryType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var repoType in repositoryTypes)
            {
                services.AddScoped(repoType);
            }

            #endregion
        }
    }
}
