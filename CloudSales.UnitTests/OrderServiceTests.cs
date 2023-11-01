using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;
using CloudSales.Api.Implementation.Repositories;
using CloudSales.Api.Implementation.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UnprocessableEntityObjectResult = Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult;

namespace CloudSales.UnitTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IPurchasedSoftwareRepository> purchasedSoftwareRepositoryMock;
        private readonly Mock<ICloudProviderRepository> cloudProviderRepositoryMock;
        private readonly IOrderService orderService;

        public OrderServiceTests()
        {
            purchasedSoftwareRepositoryMock = new Mock<IPurchasedSoftwareRepository>();
            cloudProviderRepositoryMock = new Mock<ICloudProviderRepository>();
            orderService = new OrderService(purchasedSoftwareRepositoryMock.Object, cloudProviderRepositoryMock.Object);
        }

        [Fact]
        public async Task PlaceOrderAsync_ServiceNotFound_ReturnsServiceNotFoundError()
        {
            // Arrange
            cloudProviderRepositoryMock.Setup(repo => repo.GetSoftwareServicesAsync())
                .ReturnsAsync(new List<SoftwareService> {new() {Id = 1}});
            var orderRequest = new OrderRequest {ServiceId = 2};

            // Act
            var orderResponse = await orderService.PlaceOrderAsync(1, orderRequest);

            // Assert
            Assert.False(orderResponse.Success);
            var orderError = orderResponse.OrderError as NotFoundObjectResult;
            var serviceNotFoundError = orderError?.Value as ProblemDetails; 
            Assert.Equal("SERVICE_NOT_FOUND", serviceNotFoundError?.Title);
            Assert.Equal("Service 2 not found on CCP.", serviceNotFoundError?.Detail);
        }

        [Fact]
        public async Task PlaceOrderAsync_SubscriptionAlreadyExists_ReturnsSubscriptionAlreadyExistsError()
        {
            // Arrange
            cloudProviderRepositoryMock.Setup(repo => repo.GetSoftwareServicesAsync())
                .ReturnsAsync(new List<SoftwareService> {new() {Id = 1}});
            purchasedSoftwareRepositoryMock.Setup(repo => repo.SubscriptionExists(1, 1)).ReturnsAsync(true);
            var orderRequest = new OrderRequest {ServiceId = 1};

            // Act
            var orderResponse = await orderService.PlaceOrderAsync(1, orderRequest);

            // Assert
            Assert.False(orderResponse.Success);
            var orderError = orderResponse.OrderError as UnprocessableEntityObjectResult;
            var subscriptionAlreadyExistsError = orderError?.Value as ProblemDetails; 
            Assert.Equal("SUBSCRIPTION_ALREADY_EXISTS", subscriptionAlreadyExistsError?.Title);
            Assert.Equal("Subscription already exists for service: 1 and account: 1.", subscriptionAlreadyExistsError?.Detail);
        }

        [Fact]
        public async Task PlaceOrderAsync_InvalidSubscriptionDate_ReturnsInvalidSubscriptionDateError()
        {
            // Arrange
            cloudProviderRepositoryMock.Setup(repo => repo.GetSoftwareServicesAsync())
                .ReturnsAsync(new List<SoftwareService>() {new() {Id = 1}});
            var orderRequest = new OrderRequest {ServiceId = 1, ValidTo = DateTime.UtcNow};

            // Act
            var orderResponse = await orderService.PlaceOrderAsync(1, orderRequest);

            // Assert
            Assert.False(orderResponse.Success);
            var orderError = orderResponse.OrderError as UnprocessableEntityObjectResult;
            var invalidSubscriptionDateError = orderError?.Value as ProblemDetails; 
            Assert.Equal("INVALID_SUBSCRIPTION_DATE", invalidSubscriptionDateError?.Title);
            Assert.Equal("Invalid subscription date", invalidSubscriptionDateError?.Detail);
        }

        [Fact]
        public async Task PlaceOrderAsync_SuccessfulOrder_ReturnsSubscriptionResponse()
        {
            // Arrange
            cloudProviderRepositoryMock.Setup(repo => repo.GetSoftwareServicesAsync())
                .ReturnsAsync(new List<SoftwareService> {new() {Id = 1}});
            purchasedSoftwareRepositoryMock.Setup(repo => repo.PlaceOrder(It.IsAny<PurchasedSoftware>()))
                .ReturnsAsync(true);
            var orderRequest = new OrderRequest {ServiceId = 1, ValidTo = DateTime.UtcNow.AddHours(1)};

            // Act
            var orderResponse = await orderService.PlaceOrderAsync(1, orderRequest);

            // Assert
            Assert.True(orderResponse.Success);
            Assert.IsType<SubscriptionResponse>(orderResponse.SubscriptionResponse);
        }

        [Fact]
        public async Task ValidateOrderRequestAsync_InvalidOrderRequest_ReturnsValidationErrors()
        {
            // Arrange
            var orderRequest = new OrderRequest();

            // Act
            var validationResult = await orderService.ValidateOrderRequestAsync(orderRequest);

            // Assert
            Assert.False(validationResult.IsValid);
        }

        [Fact]
        public async Task ValidateOrderRequestAsync_ValidOrderRequest_ReturnsNoValidationErrors()
        {
            // Arrange
            var orderRequest = new OrderRequest {ServiceId = 1, Quantity = 5, ValidTo = DateTime.UtcNow.AddHours(1)};

            // Act
            var validationResult = await orderService.ValidateOrderRequestAsync(orderRequest);

            // Assert
            Assert.True(validationResult.IsValid);
        }
    }
}
