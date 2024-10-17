using Moq;
using Xunit;
using TDSSWebApplication.Services;
using TDSSWebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TDSSWebApplication.Tests
{
    public class CartlogServicesTests
    {
        private readonly CartlogServices _cartlogServices;
        private readonly Mock<LinenManagementContext> _mockContext;
        private readonly Mock<ILogger<CartlogServices>> _mockLogger;

        public CartlogServicesTests()
        {
            // Setup mock context and logger
            _mockContext = new Mock<LinenManagementContext>();
            _mockLogger = new Mock<ILogger<CartlogServices>>();

            // Instantiate the service with mocked dependencies
            _cartlogServices = new CartlogServices(_mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCartLogByIdAsync_CartLogExists_ReturnsCartLog()
        {
            // Arrange
            var cartLogId = 1;
            var cartLog = new CartLog { CartLogId = cartLogId };
            var mockDbSet = new Mock<DbSet<CartLog>>();
            mockDbSet.Setup(m => m.FindAsync(cartLogId)).ReturnsAsync(cartLog);

            _mockContext.Setup(c => c.CartLogs).Returns(mockDbSet.Object);

            // Act
            var result = await _cartlogServices.GetCartLogByIdAsync(cartLogId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cartLogId, result.CartLogId);
        }

        [Fact]
        public async Task GetCartLogByIdAsync_CartLogNotFound_ReturnsNull()
        {
            // Arrange
            var cartLogId = 1;
            var mockDbSet = new Mock<DbSet<CartLog>>();
            mockDbSet.Setup(m => m.FindAsync(cartLogId)).ReturnsAsync((CartLog)null);

            _mockContext.Setup(c => c.CartLogs).Returns(mockDbSet.Object);

            // Act
            var result = await _cartlogServices.GetCartLogByIdAsync(cartLogId);

            // Assert
            Assert.Null(result);
        }
    }
}
