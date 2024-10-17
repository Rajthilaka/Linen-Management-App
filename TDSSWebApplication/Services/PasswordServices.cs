using Microsoft.AspNetCore.Identity;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;

namespace TDSSWebApplication.Services
{
    public class PasswordServices : IPasswordServices
    {
        private readonly PasswordHasher<Employee> _passwordHasher;

        public PasswordServices()
        {
            _passwordHasher = new PasswordHasher<Employee>();
        }

        //// Generate hashed password
        //public string HashPassword(Employee employee, string password)
        //{
        //    return _passwordHasher.HashPassword(employee, password);
        //}
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(Employee employee, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(employee, employee.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
