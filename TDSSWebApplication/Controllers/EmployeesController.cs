using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDSSWebApplication.DTO;
using TDSSWebApplication.IServices;
using TDSSWebApplication.Models;
using TDSSWebApplication.Services;

namespace TDSSWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly LinenManagementContext _dbcontext;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeServices _employeeServices;

        public EmployeesController(LinenManagementContext dbcontext, ILogger<EmployeesController> logger, IEmployeeServices employeeServices)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _employeeServices = employeeServices;

        }
       

        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword(int employeeId, string password)
        {
            try
            {
                await _employeeServices.GenerateAndSetPasswordAsync(employeeId, password);
                _logger.LogInformation("Successfully Set the Password");
                return Ok("Password updated successfully");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "employee with ID {employeeId} not found", employeeId);
                return NotFound(new { message = ex.Message });
            }           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error employee with ID {employeeId} not found", employeeId);
                return StatusCode(500, "Internal server error occurred.");
            }
        }
    }
}
