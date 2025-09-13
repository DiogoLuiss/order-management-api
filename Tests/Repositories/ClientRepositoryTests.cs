using Moq;
using OrderManagementApi.Data.Dapper;
using OrderManagementApi.Data.Repository;
using System.Data;
using Xunit;

namespace OrderManagementApi.Tests.Repositories
{
    public class ClientRepositoryTests
    {
        [Fact]
        public async Task DeleteClientAsync_ClientHasNoOrders_ReturnsTrue()
        {
            var dbConnectionMock = new Mock<IDbConnection>();
            var dapperMock = new Mock<IDapperWrapper>();
            var clientRepo = new ClientRepository(dbConnectionMock.Object, dapperMock.Object);

            dapperMock.Setup(d => d.ExecuteScalarAsync<int>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    null))
                .ReturnsAsync(0);

            dapperMock.Setup(d => d.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    null))
                .ReturnsAsync(1);

            var result = await clientRepo.DeleteClientAsync(1);

            Assert.True(result);
        }
    }
}
