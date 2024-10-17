using System;
using System.Collections.Generic;

namespace TDSSWebApplication.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;



    /// <summary>
    /// Used by JWT authentication to refresh access token
    /// </summary>
    public string? RefreshToken { get; set; }

    public virtual ICollection<CartLog> CartLogs { get; set; } = new List<CartLog>();
}
