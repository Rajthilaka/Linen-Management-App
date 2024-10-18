using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;

namespace TDSSWebApplication.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;

        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;            
        }

        public string CreateJwtToken(Employee employee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, employee.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:TokenExpiryInMinutes"])),
            var expiration = DateTime.Now.AddMinutes(60);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }

    
}
