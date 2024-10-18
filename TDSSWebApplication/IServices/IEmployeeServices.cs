using TDSSWebApplication.DTO;
using TDSSWebApplication.Models;
namespace TDSSWebApplication.IServices
{
    public interface IEmployeeServices
    {
       // Task<Employee> RegisterUserAsync(RegisterDto dto);
        Task GenerateAndSetPasswordAsync(int employeeId, string password);
        Task<Employee> GetUserByUsername(string name);
        Task SaveRefreshToken(int employeeId, string refreshToken);
        Task RemoveRefreshToken(int employeeId);
        Task<Employee> GetUserByRefreshToken(string refreshToken);
        

    }
}
