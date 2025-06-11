using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class InvoiceItem
    {
        public int InvoiceItemId { get; set; }
        public decimal MRP { get; set; }
        public required DateTime PrushereDate { get; set; }

        public double SalePrice { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Total { get; set; }
        public int MedId { get; set; }
        public virtual Med? Med { get; set; }
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        // Added missing property to fix CS1061  
        public string BatchId { get; set; }
    }
}
