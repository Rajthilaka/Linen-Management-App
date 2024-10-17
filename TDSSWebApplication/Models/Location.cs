using System;
using System.Collections.Generic;

namespace TDSSWebApplication.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// Valid Values = SOILED_ROOM, CLEAN_ROOM
    /// </summary>
    public string Type { get; set; } = null!;

    public virtual ICollection<CartLog> CartLogs { get; set; } = new List<CartLog>();
}
