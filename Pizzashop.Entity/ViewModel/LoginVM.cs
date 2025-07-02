using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class LoginVM
{
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage ="Invalid Email Address!")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required!")]
    public string Password { get; set; } = null!;

    public bool RememberMe {get; set;}

}
