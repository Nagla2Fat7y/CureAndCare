using Pharmacy.Models;

namespace Pharmacy.Models
{
    public class RevenueReportViewModel
    {
        public string Period { get; set; } // e.g., "2025-06-09" for day, "2025-06" for month, "2025" for year
        public decimal Sales { get; set; } // Total sales (NetTotal)
        public decimal PurchaseCost { get; set; } // Total purchase cost
        public decimal Profit { get; set; } // Sales - PurchaseCost
    }

    public class RevenueFilterViewModel
    {
        public string FilterType { get; set; } // "Day", "Month", or "Year"
        public string ReportType { get; set; } // "Sales", "PurchaseCost", or "Profit"
        public List<RevenueReportViewModel> RevenueData { get; set; } = new List<RevenueReportViewModel>();
    }
}