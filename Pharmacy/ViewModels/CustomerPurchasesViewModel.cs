namespace Pharmacy.ViewModels
{
    public class CustomerPurchasesViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public List<InvoiceViewModel> Invoices { get; set; } = new List<InvoiceViewModel>();
    }
}
