using System;
using System.Collections.Generic;

namespace TDSSWebApplication.Models;

public partial class CartLog
{
    public int CartLogId { get; set; }

    /// <summary>
    /// The delivery receipt number from the laundry vendor
    /// </summary>
    public string? ReceiptNumber { get; set; }

    /// <summary>
    /// Total weight of linen in Punds (lb) that is reported by the vendor.
    /// </summary>
    public int? ReportedWeight { get; set; }

    /// <summary>
    /// Weight of linen in pounds (lb) received by client
    /// </summary>
    public int ActualWeight { get; set; }

    public string? Comments { get; set; }

    public DateTime DateWeighed { get; set; }

    public int CartId { get; set; }

    /// <summary>
    /// Location where the cart is kept
    /// </summary>
    public int LocationId { get; set; }

    /// <summary>
    /// Employee that has weighed the cart
    /// </summary>
    public int EmployeeId { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual ICollection<CartLogDetail> CartLogDetails { get; set; } = new List<CartLogDetail>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;
   // public ICollection<CartLogDetail> LinenItems { get; set; }
}
