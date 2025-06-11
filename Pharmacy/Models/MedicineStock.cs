using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class MedicineStock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Med")]
        public int MedicineId { get; set; }

        public virtual Med? Med { get; set; }

        [MaxLength(20)]
        public string BatchId { get; set; } // رقم الدفعة

        

        [Required]
        public int Quantity { get; set; }

       
        public List<InvoiceItem> InvoiceItems { get; set; }

        // علاقات مع المشتريات عبر PurchaseItem
    }

}