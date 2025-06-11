using System.ComponentModel.DataAnnotations;

namespace Pharmacy.Models
{


    public class User
    {
        [Key]
        public int User_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string username { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [StringLength(200)]
        public string Password_Hash { get; set; }

        [Required]
        [StringLength(50)]
        public string Permission_Level { get; set; }




    }
}
