using System.ComponentModel.DataAnnotations;

namespace Pharmacy.ViewModels
{
    public class CreateMedViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Packing { get; set; }

        [Required]
        [MaxLength(100)]
        public string GenericName { get; set; }

        [Required]
        public string Cat_Name { get; set; } = null!;

        [Required]
        public string Sup_Name { get; set; } = null!;

        // ========== معلومات المخزون ==========

        [Required]
        [MaxLength(20)]
        public string BatchId { get; set; } = null!;

        [Required]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; } 

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MRP must be positive")]
        public double MRP { get; set; }

        public required DateTime PrushereDate { get; set; }

        public double SalePrice { get; set; }




    }
}
