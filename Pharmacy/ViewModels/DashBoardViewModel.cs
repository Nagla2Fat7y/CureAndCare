using System;

namespace Pharmacy.ViewModels
{
    public class DashboardViewModel
    {
        public int MedicineCount { get; set; }
        public int SupplierCount { get; set; }
        public int CustomerCount { get; set; }
        public int InvoiceCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int ExpiredCount { get; set; }
        public int ExpiredAlertCount { get; set; }
        public decimal TotalSales { get; set; } // Added for Today's Report
        public decimal TotalPurchase { get; set; } // Added for Today's Report
    }
}