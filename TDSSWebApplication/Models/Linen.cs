using System;
using System.Collections.Generic;

namespace TDSSWebApplication.Models;

public partial class Linen
{
    public int LinenId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Weight { get; set; }

    public virtual ICollection<CartLogDetail> CartLogDetails { get; set; } = new List<CartLogDetail>();
}
