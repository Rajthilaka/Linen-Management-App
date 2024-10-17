using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using TDSSWebApplication.Controllers;
using TDSSWebApplication.Services;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TDSSWebApplication.Tests
{
    public class CartlogsControllerIntegrationTests
    {
        private readonly LinenManagementContext _dbContext;
        private readonly CartlogsController _controller;
        private readonly CartlogServices _cartlogServices;
        private readonly Mock<ILogger<CartlogsController>> _mockLoggerController;
        private readonly Mock<ILogger<CartlogServices>> _mockLoggerService;

        public CartlogsControllerIntegrationTests()
        {
            // In-memory database setup
            var options = new DbContextOptionsBuilder<LinenManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new LinenManagementContext(options);

            // Mock loggers
            _mockLoggerController = new Mock<ILogger<CartlogsController>>();
            _mockLoggerService = new Mock<ILogger<CartlogServices>>();

            // Initialize actual service with in-memory database
            _cartlogServices = new CartlogServices(_dbContext, _mockLoggerService.Object);

            // Initialize controller with service
            _controller = new CartlogsController(_mockLoggerController.Object, _dbContext, _cartlogServices);
        }

        [Fact]
        public async Task GetCartLog_ValidId_ReturnsOkResult()
        {
            // Arrange: Seed data into the in-memory database
            var testCartLog = new CartLog
            {
                CartLogId = 2,
                ReceiptNumber = "12345",
                ActualWeight = 50,
                DateWeighed = System.DateTime.Now,
                EmployeeId = 2,
                CartId = 1,
                LocationId = 1,
                CartLogDetails = new List<CartLogDetail>
                {
                    new CartLogDetail { CartLogDetailId = 1, LinenId = 1, Count = 10 }
                }
            };

            _dbContext.CartLogs.Add(testCartLog);
            await _dbContext.SaveChangesAsync();

            // Act: Call the API endpoint
            var result = await _controller.GetCartLog(1);

            // Assert: Check the result
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<object>(okResult.Value);

            Assert.NotNull(returnValue);
        }

        [Fact]
        public async Task GetCartLog_InvalidId_ReturnsNotFound()
        {
            // Act: Call the API with an invalid ID
            var result = await _controller.GetCartLog(999);

            // Assert: Expect NotFound result
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
