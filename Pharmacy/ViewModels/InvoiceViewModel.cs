public class InvoiceViewModel
{
    public int InvoiceId { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string ContactNumber { get; set; }
    public int CustomerId { get; set; }
    public string EmployeeName { get; set; }
    public int? empid { get; set; }
    public string InvoiceNumber { get; set; }
    public string PaymentType { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal NetTotal { get; set; }
    public decimal Change { get; set; }
    public List<InvoiceItemViewModel> InvoiceItems { get; set; } = new();
}
