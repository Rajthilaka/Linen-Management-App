using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDSSWebApplication.DTO;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace TDSSWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartlogsController : ControllerBase
    {
        private readonly LinenManagementContext _dbcontext;
        private readonly ILogger<CartlogsController> _logger;
        private readonly ICartlogServices _cartlogServices;

        public CartlogsController(ILogger<CartlogsController> logger, LinenManagementContext dbcontext, ICartlogServices cartlogServices)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _cartlogServices = cartlogServices;
        }

        // [GET] api/cartlogs/{cartLogId}
        [HttpGet("{cartLogId}")]
        public async Task<IActionResult> GetCartLog(int cartLogId)
        {
            _logger.LogInformation("Getting CartLog with ID {CartLogId}", cartLogId);

            try
            {
                var cartLog = await _cartlogServices.GetCartLogByIdAsync(cartLogId);

                if (cartLog == null)
                {
                    _logger.LogWarning("CartLog with ID {CartLogId} not found", cartLogId);
                    return NotFound($"CartLog with ID {cartLogId} not found.");
                }

                // Mapping and creating response
                bool isClean = cartLog.Cart.Type.Equals("CLEAN", StringComparison.OrdinalIgnoreCase);
                var response = new
                {
                    cartLog.CartLogId,
                    cartLog.ReceiptNumber,
                    cartLog.ReportedWeight,
                    cartLog.ActualWeight,
                    cartLog.Comments,
                    cartLog.DateWeighed,
                    cart = new
                    {
                        cartLog.Cart.CartId,
                        cartLog.Cart.Name,
                        cartLog.Cart.Weight,
                        cartLog.Cart.Type
                    },
                    location = new
                    {
                        cartLog.Location.LocationId,
                        cartLog.Location.Name,
                        cartLog.Location.Type
                    },
                    employee = new
                    {
                        cartLog.Employee.EmployeeId,
                        cartLog.Employee.Name
                    },
                    linen = isClean && cartLog.CartLogDetails != null
                        ? cartLog.CartLogDetails.Select(li => new
                        {
                            li.CartLogDetailId,
                            li.LinenId,
                            li.Linen.Name,
                            li.Count
                        })
                        : null
                };

                _logger.LogInformation("Successfully fetched CartLog with ID {CartLogId}", cartLogId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching CartLog with ID {CartLogId}", cartLogId);
                return StatusCode(500, "Internal server error occurred.");
            }
        }

        // [GET] api/cartlogs
        [HttpGet]
        public async Task<IActionResult> GetCartLogs([FromQuery] string? cartType, [FromQuery] int? locationId, [FromQuery] int? employeeId)
        {
            _logger.LogInformation("Fetching filtered CartLogs");
            try
            {
                var cartLogs = await _cartlogServices.GetFilteredCartLogsAsync(cartType, locationId, employeeId);

                var response = cartLogs.Select(cartLog => new
                {
                    cartLog.CartLogId,
                    cartLog.ReceiptNumber,
                    cartLog.ReportedWeight,
                    cartLog.ActualWeight,
                    cartLog.Comments,
                    cartLog.DateWeighed,
                    cart = new
                    {
                        cartLog.Cart.CartId,
                        cartLog.Cart.Name,
                        cartLog.Cart.Weight,
                        cartLog.Cart.Type
                    },
                    location = new
                    {
                        cartLog.Location.LocationId,
                        cartLog.Location.Name,
                        cartLog.Location.Type
                    },
                    employee = new
                    {
                        cartLog.Employee.EmployeeId,
                        cartLog.Employee.Name
                    },
                    linen = cartLog.CartLogDetails.Select(li => new
                    {
                        li.CartLogDetailId,
                        li.LinenId,
                        li.Linen.Name,
                        li.Count
                    })
                });

                _logger.LogInformation("Successfully fetched filtered CartLogs");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching CartLogs");
                return StatusCode(500, "Internal server error occurred.");
            }
        }

        // [POST] api/cartlogs/upsert
        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertCartLog([FromBody] CartLogDto cartLogDto)
        {
            _logger.LogInformation("Upserting CartLog");
            try
            {
                var result = await _cartlogServices.UpsertCartLogAsync(cartLogDto);

                _logger.LogInformation("Successfully upserted CartLog");
                
                return Ok(new { cartLogs = result });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access during upsert");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting CartLog");
                return BadRequest("An error occurred while processing your request.");
            }
        }

        // [DELETE] api/cartlogs/{cartLogId}
        [HttpDelete("{cartLogId:int}")]
        public async Task<IActionResult> DeleteCartLog(int cartLogId)
        {
            _logger.LogInformation("Attempting to delete CartLog with ID {CartLogId}", cartLogId);
            try
            {
                // Retrieve the employee ID from the JWT token
               // int employeeId = int.Parse(User.FindFirst("id").Value);
                await _cartlogServices.DeleteCartLogAsync(cartLogId, 2); // Assuming 2 is employeeId
                _logger.LogInformation("Successfully deleted CartLog with ID {CartLogId}", cartLogId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "CartLog with ID {CartLogId} not found", cartLogId);
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access when deleting CartLog with ID {CartLogId}", cartLogId);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CartLog with ID {CartLogId}", cartLogId);
                return StatusCode(500, "Internal server error occurred.");
            }
        }
    }
}
