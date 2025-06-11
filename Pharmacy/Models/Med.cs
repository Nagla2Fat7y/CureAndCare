using Pharmacy.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Med
{
    [Key]
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
    [MaxLength(10)]
    public required DateTime ExpiryDate { get; set; }

    public required DateTime PrushereDate { get; set; }

    public double SalePrice { get; set; }

    public double MRP { get; set; }

    // Supplier Relation 
    [ForeignKey("Supplier")]
    public int Sup_ID { get; set; }
    public virtual Supplier? Supplier { get; set; }

    // Category Relation
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }

    // ✅ List of MedicineStock
    public virtual ICollection<MedicineStock> MedicineStocks { get; set; }
}
