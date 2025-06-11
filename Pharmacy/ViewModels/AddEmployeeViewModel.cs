using System.ComponentModel.DataAnnotations;

public class AddEmployeeViewModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public int Salary { get; set; }

    public string Address { get; set; }

    public string Phone { get; set; }

    public string Shift { get; set; }
}
