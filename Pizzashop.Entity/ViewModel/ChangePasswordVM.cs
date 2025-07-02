using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class ChangePasswordVM
{
    public string? Email {get; set;}

    [Display(Name = "New Password")]
    [Required(ErrorMessage = "New Password is required!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",  ErrorMessage = "Enter Strong Password!")]
    public string NewPassword {get; set;} = null!;

    [Display(Name = "Confirm New Password")]
    [Required(ErrorMessage = "Confirm New Password is required!")]
    [Compare("NewPassword", ErrorMessage = "New Password and Confirm New Password must be same.")]
    public string ConfirmPassword {get; set;} = null!;

    [Display(Name = "Current Password")]
    [Required(ErrorMessage = "Current Password is required!")]
    public string CurrentPassword {get; set;} = null!;
}
