using CloudSales.Api.Controllers;
using CloudSales.Api.Dtos;
using CloudSales.Api.Implementation.Domain;
using CloudSales.Api.Implementation.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CloudSales.UnitTests
{
    public class CustomerControllerTests
    {
        [Fact]
        public async Task GetCustomerByNumber_ValidCustomerNumber_ReturnsOkResult()
        {
            // Arrange
            var customerNumber = "CN000000001214";
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            var controller = new CustomerController(null, mockCustomerRepository.Object);

            var expectedCustomer = new Customer
            {
                Id = 1,
                Name = "New company",
                Email = "some@new-company.com",
                Type = CustomerType.Direct
            };
            mockCustomerRepository.Setup(repo => repo.GetCustomerByNumber(customerNumber)).ReturnsAsync(expectedCustomer);

            // Act
            var result = await controller.GetCustomerByNumber(customerNumber);

            // Assert
            var okResult = result as OkObjectResult;
            var customerDto = Assert.IsType<CustomerResponse>(okResult.Value);
            Assert.Equal(expectedCustomer.CustomerIdentifier, customerDto.CustomerIdentifier);
            Assert.Equal(expectedCustomer.Name, customerDto.Name);
            Assert.Equal(expectedCustomer.Email, customerDto.Email);
            Assert.Equal(expectedCustomer.Type.ToString(), customerDto.Type);
        }

        [Fact]
        public async Task GetCustomerByNumber_InvalidCustomerNumber_ReturnsNotFoundResult()
        {
            // Arrange
            var customerNumber = "invalid-customer-number";
            var mockCustomerRepository = new Mock<ICustomerRepository>();
            var controller = new CustomerController(null, mockCustomerRepository.Object);

            // Mock the repository to return null (customer not found)
            mockCustomerRepository.Setup(repo => repo.GetCustomerByNumber(customerNumber))
                .ReturnsAsync((Customer?) null);

            // Act
            var result = await controller.GetCustomerByNumber(customerNumber);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAccountsByCustomerNumber_ValidCustomerNumber_ReturnsOkResult()
        {
            // Arrange
            var customerNumber = "CN000000001214";
            var mockAccountRepository = new Mock<IAccountRepository>();
            var controller = new CustomerController(mockAccountRepository.Object, null);

            var expectedAccounts = new List<Account>()
            {
                new()
                {
                    Id = 1, Name = "Business Account", CustomerId = 1
                },
                new()
                {
                    Id = 2, Name = "Business Plus Account", CustomerId = 1
                }
            };
            mockAccountRepository.Setup(repo => repo.GetAccountsByCustomer(customerNumber))
                .ReturnsAsync(expectedAccounts);

            // Act
            var result = await controller.GetAccountsByCustomerNumber(customerNumber);

            // Assert
            var okResult = result as OkObjectResult;
            var response = Assert.IsType<Response<List<AccountResponse>>>(okResult.Value);
            Assert.Equal(expectedAccounts.Count, response.Data.Count);
            Assert.Equal(expectedAccounts[0].Name, response.Data[0].Name);
            Assert.Equal(expectedAccounts[0].CustomerId, response.Data[0].CustomerId);
        }

        [Fact]
        public async Task GetAccountsByCustomerNumber_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var customerNumber = "12345";
            var mockAccountRepository = new Mock<IAccountRepository>();
            var controller = new CustomerController(mockAccountRepository.Object, null);

            // Mock the repository to throw an exception
            mockAccountRepository.Setup(repo => repo.GetAccountsByCustomer(customerNumber)).Throws(new Exception("Test exception"));

            // Act
            var result = await controller.GetAccountsByCustomerNumber(customerNumber);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
