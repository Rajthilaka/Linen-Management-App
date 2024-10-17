namespace TDSSWebApplication.DTO
{
    public class CartLogDto
    {
        public int? CartLogId { get; set; }
        public string? ReceiptNumber { get; set; }
        public int? ReportedWeight { get; set; }
        public int ActualWeight { get; set; }
        public string? Comments { get; set; }
        public DateTime DateWeighed { get; set; }
        public int CartId { get; set; }
        public int LocationId { get; set; }
        public int EmployeeId { get; set; }
        public List<CartLogDetailDto> Linen { get; set; }
    }
}
