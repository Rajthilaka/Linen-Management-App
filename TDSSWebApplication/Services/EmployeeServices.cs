using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDSSWebApplication.DTO;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;


namespace TDSSWebApplication.Services
{
    public class EmployeeServices : IEmployeeServices
    {
        private readonly LinenManagementContext _dbcontext;
        private readonly PasswordServices _passwordServices;
        private readonly ILogger<EmployeeServices> _logger;


        public EmployeeServices(LinenManagementContext dbcontext, ILogger<EmployeeServices> logger)
        {
            _dbcontext = dbcontext;
            _passwordServices = new PasswordServices();
            _logger = logger;
        }

        // Get the user by username from the database
        public async Task<Employee> GetUserByUsername(string name)
        {
            return await _dbcontext.Employees.SingleOrDefaultAsync(u => u.Name == name);
        }

        public async Task<Employee> GetUserByRefreshToken(string refreshToken)
        {
            return await _dbcontext.Employees.FirstOrDefaultAsync(e => e.RefreshToken == refreshToken);
        }
        // Save the refresh token in the database with the user
        public async Task SaveRefreshToken(int employeeId, string refreshToken)
        {
            var user = await _dbcontext.Employees.FindAsync(employeeId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
               
                await _dbcontext.SaveChangesAsync();
            }
        }

        public async Task RemoveRefreshToken(int employeeId)
        {
            var user = await _dbcontext.Employees.FindAsync(employeeId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Assuming you have a field for RefreshToken in the Employee entity
            user.RefreshToken = null;

            _dbcontext.Employees.Update(user);
            await _dbcontext.SaveChangesAsync();
        }       

        public async Task GenerateAndSetPasswordAsync(int employeeId, string password)
        {
            try
            {
                // Fetch the user from the database
                _logger.LogInformation($"Trace: Generate and Set Password with ID {employeeId}");
                var user = await _dbcontext.Employees.FindAsync(employeeId);
                if (user == null)
                {
                    _logger.LogWarning($"Warning: Employee with ID {employeeId} not found.");
                    throw new KeyNotFoundException("employeeId not found.");
                }
                else 
                {
                    // Hash the password
                    user.Password = _passwordServices.HashPassword(password);

                    // Save changes to the database
                    _dbcontext.Employees.Update(user);
                    await _dbcontext.SaveChangesAsync();
                }
                _logger.LogInformation("Trace: Successfully Set Password");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: Failed to Set Password with ID {employeeId}");
                throw;
            }

        }
    }
}
