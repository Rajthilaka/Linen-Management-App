using TDSSWebApplication.DTO;
using TDSSWebApplication.Models;

namespace TDSSWebApplication.IServices
{
    public interface ICartlogServices
    {
        Task<CartLog> GetCartLogByIdAsync(int cartLogId);
        Task<List<CartLog>> GetFilteredCartLogsAsync(string? cartType, int? locationId, int? employeeId);
        Task<CartLogDto> UpsertCartLogAsync(CartLogDto cartLogDto);
        Task<bool> DeleteCartLogAsync(int cartLogId, int employeeId);
       

    }
}
