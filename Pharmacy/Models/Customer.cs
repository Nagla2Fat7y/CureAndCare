using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class Customer
    {

        [Key]
        public int Customer_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Add { get; set; }

        public int Loyalty_Points { get; set; }

        public ICollection<Invoice>? Invoices { get; set; } = new HashSet<Invoice>();
    }
}
