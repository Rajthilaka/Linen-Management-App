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
        private readonly ILogger<AuthController> _logger;
        private readonly IPasswordServices _passwordServices;
        private readonly IEmployeeServices _employeeServices;
        private readonly ITokenServices _tokenServices;

        public AuthController(LinenManagementContext dbcontext, ILogger<AuthController> logger, IPasswordServices passwordServices, IEmployeeServices employeeServices, ITokenServices tokenServices)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _passwordServices = passwordServices;
            _employeeServices = employeeServices;
            _tokenServices = tokenServices;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try 
            {
                _logger.LogInformation("Jwt token authentication login");
                var user = await _employeeServices.GetUserByUsername(loginDto.Name);

            if (user == null || !_passwordServices.VerifyPassword(user.Password, loginDto.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var jwtToken = _tokenServices.CreateJwtToken(user);
            var refreshToken = _tokenServices.CreateRefreshToken();

            // Store refresh token with user
            await _employeeServices.SaveRefreshToken(user.EmployeeId, refreshToken);

            return Ok(new { Token = jwtToken, RefreshToken = refreshToken });

            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation(ex, "Login - invalid credentials");
                return BadRequest("A required parameter was null.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Login Error");
                return StatusCode(500, "Oops! Something went wrong, please call support.");
            }
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                _logger.LogInformation("Jwt Authentication Logout");
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {                   
                    return Unauthorized();
                }

                if (!int.TryParse(userId, out int employeeId))
                {
                    return BadRequest("Invalid user ID format.");
                }
                // Remove refresh token
                await _employeeServices.RemoveRefreshToken(employeeId);
                return Ok();
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation(ex, "Logout Error");

                return BadRequest("A required parameter was null.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Logout Error");
                return StatusCode(500, "Oops! Something went wrong, please call support.");
            }

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshDto tokenRefreshDto)
        {
            try
            {
                _logger.LogInformation("Jwt Athentication TokenRefreh");
                var user = await _employeeServices.GetUserByRefreshToken(tokenRefreshDto.RefreshToken);

                if (user == null)
                {
                    return Unauthorized();
                }

                var jwtToken = _tokenServices.CreateJwtToken(user);
                var newRefreshToken = _tokenServices.CreateRefreshToken();

                // Update refresh token
                await _employeeServices.SaveRefreshToken(user.EmployeeId, newRefreshToken);

                return Ok(new { Token = jwtToken, RefreshToken = newRefreshToken });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation(ex, "RefreshToken Error");
                return BadRequest("A required parameter was null.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "RefreshToken Error");
                return StatusCode(500, "Oops! Something went wrong, please call support.");
            }

        }

      
    }
}
