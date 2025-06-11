using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; }

      
        [Required]
        public DateTime InvoiceDate { get; set; }

        [MaxLength(50)]
        public string PaymentType { get; set; } // e.g., "Cash Payment"

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalDiscount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal NetTotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PaidAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Change { get; set; } // Amount returned to the customer
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("emp")]
        public string empid { get; set; }
        public virtual emp? emp { get; set; }

        // One-to-Many relationship with InvoiceItem
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    }
}
