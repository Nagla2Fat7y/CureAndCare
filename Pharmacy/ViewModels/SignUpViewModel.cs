using System.ComponentModel.DataAnnotations;

namespace Pharmacy.ViewModels
{
    public class SignUpViewModel
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="The Password is not match. ")]
        [Display(Name = "Confirm Password")]
        public required string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "You must agree to the terms.")]
        public bool AgreeToTerms { get; set; }

    }
}
