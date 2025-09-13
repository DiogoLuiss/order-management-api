using System.Data;
using Moq;
using OrderManagementApi.Data.Dapper;
using OrderManagementApi.Data.Repository;
using OrderManagementApi.Exceptions;
using OrderManagementApi.Models;
using Xunit;

namespace OrderManagementApi.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task DeleteProductAsync_WhenProductHasOrders_ShouldThrowBadRequestException()
        {
            var dbConnectionMock = new Mock<IDbConnection>();
            var dapperMock = new Mock<IDapperWrapper>();

            var repo = new ProductRepository(dbConnectionMock.Object, dapperMock.Object);

            dapperMock.Setup(d => d.ExecuteScalarAsync<int>(It.IsAny<string>(), new { ProductId = 1}, null))
                      .ReturnsAsync(2);

            await Assert.ThrowsAsync<BadRequestException>(
                () => repo.DeleteProductAsync(1));
        }

        [Fact]
        public async Task DeleteProductAsync_WhenProductHasNoOrders_ShouldDeleteAndReturnTrue()
        {
            var dbConnectionMock = new Mock<IDbConnection>();
            var dapperMock = new Mock<IDapperWrapper>();

            var repo = new ProductRepository(dbConnectionMock.Object, dapperMock.Object);

            dapperMock.Setup(d => d.ExecuteScalarAsync<int>(It.IsAny<string>(), null, null))
                      .ReturnsAsync(0);

            dapperMock.Setup(d => d.ExecuteAsync(It.IsAny<string>(), new { Id = 1 }, null))
                      .ReturnsAsync(1);

            var result = await repo.DeleteProductAsync(1);
            Assert.True(result);
        }


    }
}
