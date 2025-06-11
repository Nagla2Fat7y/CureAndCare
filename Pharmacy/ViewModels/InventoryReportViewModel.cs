using System;
using System.Collections.Generic;

namespace Pharmacy.ViewModels
{
    public class InventoryReportViewModel
    {
        public List<StockItem> StockItems { get; set; } = new List<StockItem>();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class StockItem
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string GenericName { get; set; }
        public string Packing { get; set; }
        public DateTime ExpiryDate { get; set; }

        public double MRP { get; set; }
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
        public string BatchId { get; set; }
        public int Quantity { get; set; }
        public required DateTime PrushereDate { get; set; }

        public double SalePrice { get; set; }
    }
}