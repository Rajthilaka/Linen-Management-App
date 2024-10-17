using Microsoft.EntityFrameworkCore;
using TDSSWebApplication.Models;
using TDSSWebApplication.IServices;
using TDSSWebApplication.DTO;
using Microsoft.Extensions.Logging;

namespace TDSSWebApplication.Services
{
    public class CartlogServices : ICartlogServices
    {
        private readonly LinenManagementContext _dbcontext;
        private readonly ILogger<CartlogServices> _logger;

        public CartlogServices(LinenManagementContext dbcontext, ILogger<CartlogServices> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        public async Task<CartLog> GetCartLogByIdAsync(int cartLogId)
        {
            try
            {
                _logger.LogInformation($"Trace: Retrieving CartLog with ID {cartLogId}");
                var cartLog = await _dbcontext.CartLogs
                    .Include(cl => cl.Cart)
                    .Include(cl => cl.Employee)
                    .Include(cl => cl.Location)
                    .Include(cl => cl.CartLogDetails)
                        .ThenInclude(cld => cld.Linen)
                    .SingleOrDefaultAsync(cl => cl.CartLogId == cartLogId);

                if (cartLog == null)
                {
                    _logger.LogWarning($"Warning: CartLog with ID {cartLogId} not found.");
                }

                return cartLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: Failed to retrieve CartLog with ID {cartLogId}");
                throw;
            }
        }

        public async Task<List<CartLog>> GetFilteredCartLogsAsync(string? cartType, int? locationId, int? employeeId)
        {
            try
            {
                _logger.LogInformation("Trace: Retrieving filtered CartLogs");
                var query = _dbcontext.CartLogs
                    .Include(cl => cl.Cart)
                    .Include(cl => cl.Employee)
                    .Include(cl => cl.Location)
                    .Include(cl => cl.CartLogDetails)
                        .ThenInclude(cld => cld.Linen)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(cartType))
                {
                    query = query.Where(cl => cl.Cart.Type.ToLower() == cartType.ToLower());
                }

                if (locationId.HasValue)
                {
                    query = query.Where(cl => cl.LocationId == locationId.Value);
                }

                if (employeeId.HasValue)
                {
                    query = query.Where(cl => cl.EmployeeId == employeeId.Value);
                }

                var cartLogs = await query.OrderByDescending(cl => cl.DateWeighed).ToListAsync();
                _logger.LogInformation("Trace: Successfully retrieved filtered CartLogs");
                return cartLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: Failed to retrieve filtered CartLogs");
                throw;
            }
        }

        public async Task<CartLogDto> UpsertCartLogAsync(CartLogDto cartLogDto)
        {
            try
            {
                _logger.LogInformation("Trace: Upserting CartLog");
                var existingCartLog = await _dbcontext.CartLogs
                    .Include(cl => cl.CartLogDetails)
                    .FirstOrDefaultAsync(cl => cl.CartLogId == cartLogDto.CartLogId);

                if (existingCartLog != null)
                {
                    if (existingCartLog.EmployeeId != cartLogDto.EmployeeId)
                    {
                        _logger.LogWarning("Unauthorized access during CartLog update.");
                        throw new UnauthorizedAccessException("Only the employee who created the cart log can edit it.");
                    }

                    existingCartLog.ReceiptNumber = cartLogDto.ReceiptNumber;
                    existingCartLog.ReportedWeight = cartLogDto.ReportedWeight;
                    existingCartLog.ActualWeight = cartLogDto.ActualWeight;
                    existingCartLog.Comments = cartLogDto.Comments;
                    existingCartLog.DateWeighed = cartLogDto.DateWeighed;

                    // Validation to ensure all LinenIds exist
                    foreach (var detail in cartLogDto.Linen)
                    {
                        var linenExists = await _dbcontext.Linens.AnyAsync(l => l.LinenId == detail.LinenId);
                        if (!linenExists)
                        {
                            throw new Exception($"Linen with ID {detail.LinenId} does not exist.");
                        }
                    }
                    //foreach (var detailDto in cartLogDto.Linen)
                    //{
                    //    var existingDetail = existingCartLog.CartLogDetails
                    //        .FirstOrDefault(d => d.CartLogDetailId == detailDto.CartLogDetailId);

                    //    if (existingDetail != null)
                    //    {
                    //        existingDetail.LinenId = detailDto.LinenId;
                    //        existingDetail.Count = detailDto.Count;
                    //    }
                    //    else
                    //    {
                    //        existingCartLog.CartLogDetails.Add(new CartLogDetail
                    //        {
                    //            LinenId = detailDto.LinenId,
                    //            Count = detailDto.Count
                    //        });
                    //    }
                    //}
                }
                else
                {
                    var newCartLog = new CartLog
                    {
                        ReceiptNumber = cartLogDto.ReceiptNumber,
                        ReportedWeight = cartLogDto.ReportedWeight,
                        ActualWeight = cartLogDto.ActualWeight,
                        Comments = cartLogDto.Comments,
                        DateWeighed = cartLogDto.DateWeighed,
                        CartId = cartLogDto.CartId,
                        LocationId = cartLogDto.LocationId,
                        EmployeeId = cartLogDto.EmployeeId,
                        CartLogDetails = cartLogDto.Linen.Select(l => new CartLogDetail
                        {
                            LinenId = l.LinenId,
                            Count = l.Count
                        }).ToList()
                    };

                    await _dbcontext.CartLogs.AddAsync(newCartLog);
                }

                await _dbcontext.SaveChangesAsync();
                _logger.LogInformation("Trace: Successfully upserted CartLog");
                return cartLogDto;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access during CartLog upsert.");
                throw;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: Failed to upsert CartLog");
                throw;
            }
        }

        public async Task<bool> DeleteCartLogAsync(int cartLogId, int employeeId)
        {
            try
            {
                _logger.LogInformation($"Trace: Deleting CartLog with ID {cartLogId}");
                var cartLog = await _dbcontext.CartLogs
                    .Include(cl => cl.CartLogDetails)
                    .FirstOrDefaultAsync(cl => cl.CartLogId == cartLogId);

                if (cartLog == null)
                {
                    _logger.LogWarning($"Warning: CartLog with ID {cartLogId} not found.");
                    throw new KeyNotFoundException("CartLogId not found.");
                }

                if (cartLog.EmployeeId != employeeId)
                {
                    _logger.LogWarning("Unauthorized access during CartLog deletion.");
                    throw new UnauthorizedAccessException("You are not authorized to delete this CartLog.");
                }

                _dbcontext.CartLogDetails.RemoveRange(cartLog.CartLogDetails);
                _dbcontext.CartLogs.Remove(cartLog);
                await _dbcontext.SaveChangesAsync();

                _logger.LogInformation($"Trace: Successfully deleted CartLog with ID {cartLogId}");
                return true;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Warning: Failed to delete CartLog with ID {cartLogId}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, $"Unauthorized access during CartLog deletion for CartLog ID {cartLogId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: Failed to delete CartLog with ID {cartLogId}");
                throw;
            }
        }
    }
}
