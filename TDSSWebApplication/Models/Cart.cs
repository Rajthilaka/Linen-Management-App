using System;
using System.Collections.Generic;

namespace TDSSWebApplication.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// Weight in Pounds (lb)
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Valid Values = SOILED, CLEAN
    /// </summary>
    public string? Type { get; set; }

    public virtual ICollection<CartLog> CartLogs { get; set; } = new List<CartLog>();
}
