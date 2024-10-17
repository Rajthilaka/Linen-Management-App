using TDSSWebApplication.DTO;
using TDSSWebApplication.Models;
namespace TDSSWebApplication.IServices
{
    public interface IEmployeeServices
    {
       // Task<Employee> RegisterUserAsync(RegisterDto dto);
        Task GenerateAndSetPasswordAsync(int employeeId, string password);
        
    }
}
