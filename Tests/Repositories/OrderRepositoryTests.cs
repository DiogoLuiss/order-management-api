using Moq;
using OrderManagementApi.Data.Dapper;
using OrderManagementApi.Data.Repository;
using OrderManagementApi.Exceptions;
using System.Data;
using Xunit;

namespace OrderManagementApi.Tests.Repositories
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task UpdateOrderStatusAsync_OrderDoesNotExist_ThrowsBadRequest()
        {
            var dbConnectionMock = new Mock<IDbConnection>();
            var dapperMock = new Mock<IDapperWrapper>();

            var productRepoMock = new Mock<ProductRepository>(dbConnectionMock.Object, dapperMock.Object);
            var clientRepoMock = new Mock<ClientRepository>(dbConnectionMock.Object, dapperMock.Object);
            var notificationRepoMock = new Mock<NotificationRepository>(dbConnectionMock.Object);
            var statusRepoMock = new Mock<OrderStatusRepository>(dbConnectionMock.Object);

            dapperMock.Setup(d => d.ExecuteScalarAsync<int>(It.IsAny<string>(), null, null))
                      .ReturnsAsync(0);

            var orderRepo = new OrderRepository(
                dbConnectionMock.Object,
                productRepoMock.Object,
                clientRepoMock.Object,
                notificationRepoMock.Object,
                statusRepoMock.Object,
                dapperMock.Object);

            await Assert.ThrowsAsync<BadRequestException>(
                () => orderRepo.UpdateOrderStatusAsync(1, 2));
        }
    }
}
