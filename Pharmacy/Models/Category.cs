using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{
    public class Category
    {
        [Key]
        public int Cat_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Cat_Name { get; set; } = null!;

        [MaxLength(255)]
        public string? Description { get; set; }
        public ICollection<Med> Meds { get; set; } = new HashSet<Med>();



       
    }
}
