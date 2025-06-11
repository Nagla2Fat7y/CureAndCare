using Pharmacy.Models;

namespace Pharmacy.ViewModels
{



    public class SalesReportViewModel
    {
        public List<SalesItem> SalesItems { get; set; } = new List<SalesItem>();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class SalesItem
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public decimal NetTotal { get; set; }
    }
}
