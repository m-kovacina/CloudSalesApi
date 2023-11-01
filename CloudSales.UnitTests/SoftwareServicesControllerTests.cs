using CloudSales.Api.Controllers;
using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;
using CloudSales.Api.Implementation.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CloudSales.UnitTests
{
    public class SoftwareServicesControllerTests
    {
        private readonly SoftwareServicesController softwareServicesController;
        private readonly Mock<ICloudProviderRepository> cloudProviderRepositoryMock;

        public SoftwareServicesControllerTests()
        {
            cloudProviderRepositoryMock = new Mock<ICloudProviderRepository>();
            softwareServicesController = new SoftwareServicesController(cloudProviderRepositoryMock.Object);
        }

        [Fact]
        public async Task GetSoftwareServices_Success_ReturnsOk()
        {
            // Arrange
            var expectedServices = new List<SoftwareService>
            {
                new() { Id = 1, Name = "Service 1" },
                new() { Id = 2, Name = "Service 2" },
            };

            cloudProviderRepositoryMock.Setup(repo => repo.GetSoftwareServicesAsync()).ReturnsAsync(expectedServices);

            // Act
            var result = await softwareServicesController.GetSoftwareServices();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var response = Assert.IsType<Response<List<SoftwareService>>>(okResult?.Value);
            Assert.Equal(expectedServices.Count, response.Data.Count);
            Assert.Equal(expectedServices, response.Data);
        }

        [Fact]
        public async Task GetSoftwareServices_Error_ReturnsInternalServerError()
        {
            // Arrange
            cloudProviderRepositoryMock.Setup(repo => repo.GetSoftwareServicesAsync()).Throws(new Exception("An error occurred"));

            // Act
            var result = await softwareServicesController.GetSoftwareServices();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}