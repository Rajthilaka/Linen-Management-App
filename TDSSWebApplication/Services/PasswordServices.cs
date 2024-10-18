using Microsoft.AspNetCore.Identity;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;

namespace TDSSWebApplication.Services
{
    public class PasswordServices : IPasswordServices
    {
       
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
