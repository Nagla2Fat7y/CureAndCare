namespace Pharmacy.ViewModels
{
    public class EmployeeInvoicesViewModel
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<InvoiceViewModel> Invoices { get; set; } = new List<InvoiceViewModel>();
    }
}
