﻿using System.ComponentModel.DataAnnotations;

namespace Pharmacy.ViewModels
{
    public class SignInViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }


        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
