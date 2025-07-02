using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class ResetPasswordVM
{
    [Required]
    public Guid Id {get; set;}

    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8, ErrorMessage = "Minimum 8 characters required!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",  ErrorMessage = "Enter Strong Password")]
    public string Password {get; set; } = null!;

    [Compare("Password", ErrorMessage = "Password and Confirmation Password must be same.")]
    [Required(ErrorMessage = "Confirm Password is required!")]
    public string ConfirmPassword {get; set;} = null!;

    [Required]
    public Guid Token {get; set;}
}
