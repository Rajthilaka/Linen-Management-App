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
        //// Call IEmployeeServices in Register method
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        //{
        //    var user = await _employeeServices.RegisterUserAsync(dto);

        //    if (user == null)
        //    {
        //        return BadRequest("User registration failed");
        //    }

        //    return Ok("User registered successfully");
        //}

        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword(int employeeId, string password)
        {
            await _employeeServices.GenerateAndSetPasswordAsync(employeeId, password);
            return Ok("Password updated successfully");
        }
    }
}
