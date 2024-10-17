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


        public EmployeeServices(LinenManagementContext dbcontext)
        {
            _dbcontext = dbcontext;
            _passwordServices = new PasswordServices();
        }
       
        //public async Task<Employee> RegisterUserAsync(RegisterDto dto)
        //{
        //    // Create a new user
        //    var user = new Employee
        //    {
        //        Name = dto.Name,
        //        Password = _passwordServices.HashPassword(new Employee(), dto.Password) // Hash the password
        //    };

        //    // Save to the database
        //    _dbcontext.Employees.Add(user);
        //    await _dbcontext.SaveChangesAsync();

        //    return user;
        //}

        public async Task GenerateAndSetPasswordAsync(int employeeId, string password)
        {
            // Fetch the user from the database
            var user = await _dbcontext.Employees.FindAsync(employeeId);
            if (user != null)
            {
                // Hash the password
                user.Password = _passwordServices.HashPassword(password);

                // Save changes to the database
                _dbcontext.Employees.Update(user);
                await _dbcontext.SaveChangesAsync();
            }
        }
    }
}
