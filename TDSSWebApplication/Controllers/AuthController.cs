using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TDSSWebApplication.DTO;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;
using TDSSWebApplication.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TDSSWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LinenManagementContext _dbcontext;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IPasswordServices _passwordServices;
        private readonly TokenServices _tokenServices;

        public AuthController(LinenManagementContext dbcontext, ILogger<EmployeesController> logger, IPasswordServices passwordServices, TokenServices tokenServices)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _passwordServices = passwordServices;
            _tokenServices= tokenServices;

        }
        // [POST] api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _dbcontext.Employees.FirstOrDefaultAsync(u => u.Name == dto.Name);

        //    var Password = await _dbcontext.Employees
        //.Where(e => e.Name == dto.Name)
        //.Select(e => e.Password)
        //.FirstOrDefaultAsync();
           
        //    byte[] decodedPassword = Convert.FromBase64String(Password);

            //if (user == null || !_passwordServices.VerifyPassword(user, dto.Password))
            //    return Unauthorized("Invalid credentials");

            var token = _tokenServices.GenerateJwtToken(user);
            var refreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _dbcontext.SaveChangesAsync();

            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        // [POST] api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _dbcontext.Employees.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
                return Unauthorized();

            user.RefreshToken = null;
            await _dbcontext.SaveChangesAsync();

            return Ok("User logged out successfully");
        }

        // [POST] api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshDto dto)
        {
            var principal = _tokenServices.GetPrincipalFromExpiredToken(dto.Token);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _dbcontext.Employees.FirstOrDefaultAsync(u => u.EmployeeId == int.Parse(userId));
            //if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            if (user == null || user.RefreshToken != dto.RefreshToken)
                return Unauthorized("Invalid refresh token");

            var newToken = _tokenServices.GenerateJwtToken(user);
            var newRefreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
          //  user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _dbcontext.SaveChangesAsync();

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }
    }
}
