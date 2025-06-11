using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy.Models
{
    public class Supplier
    {
        [Key]
        public int Sup_ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Sup_Name { get; set; } = null!;

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string? Sup_Mail { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string? Sup_Phone { get; set; }

        [ForeignKey(nameof(Med))]
        public int Med_ID { get; set; }
        public virtual ICollection<Med> Meds { get; set; } = new HashSet<Med>();

    }
}
