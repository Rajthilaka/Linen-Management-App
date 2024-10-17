using System;
using System.Collections.Generic;

namespace TDSSWebApplication.Models;

public partial class CartLogDetail
{
    public int CartLogDetailId { get; set; }

    public int CartLogId { get; set; }

    public int LinenId { get; set; }

    public int Count { get; set; }

    public virtual CartLog CartLog { get; set; } = null!;

    public virtual Linen Linen { get; set; } = null!;
    
}
