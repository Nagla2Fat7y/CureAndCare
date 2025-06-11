using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.ViewModels
{
    public class ProfileViewModel
    {
        public string User_Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username must not exceed 50 characters")]
        public string username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Must be a valid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(200, ErrorMessage = "Password must not exceed 200 characters")]
        public string Password_Hash { get; set; }

        [Required(ErrorMessage = "Permission level is required")]
        [StringLength(50, ErrorMessage = "Permission level must not exceed 50 characters")]
        public string Permission_Level { get; set; } // Can be populated from a dropdown (e.g., Admin, Employee)

        public int Salary { get; set; }

        [StringLength(200, ErrorMessage = "Address must not exceed 200 characters")]
        public string Address { get; set; }

        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 digits")]
        [Phone(ErrorMessage = "Must be a valid phone number")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Shift must not exceed 50 characters")]
        public string Shift { get; set; } // Can be set to values like: Morning, Evening, Night

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Hire date is required")]
        public DateTime HireDate { get; set; } // Default set to today's date on creation (03:34 AM EEST on Monday, June 09, 2025)

        [DataType(DataType.DateTime)]
        public DateTime LoginTimestamp { get; set; } // Updated automatically on login
    }
}