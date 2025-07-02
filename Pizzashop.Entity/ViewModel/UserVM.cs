using System.ComponentModel.DataAnnotations;
namespace Pizzashop.Entity.ViewModel;

public class UserVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "First Name is required!")]
    [RegularExpression("^[a-zA-Z]{1,}$", ErrorMessage = "First name should only contain alphabets!")]
    [StringLength(20)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required!")]
    [RegularExpression("^[a-zA-Z]{1,20}$", ErrorMessage = "Last name should only contain alphabets!")]
    [StringLength(20)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "User Name is required!")]
    [StringLength(20)]
    [Display(Name = "User Name")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid Email Address!")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = " Password is required!")]
    [MinLength(8, ErrorMessage = "Minimum 8 characters required!")]
    [StringLength(30)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",  ErrorMessage = "Enter Strong Password")]
    public string Password { get; set; } = null!;

    public string? ProfileImage { get; set; }

    public string? Address { get; set; }

    [Required(ErrorMessage = "City is required!")]
    [Display(Name = "City")]
    public Guid CityId { get; set; }

    [Required(ErrorMessage = "State is required!")]
    [Display(Name = "State")]
    public Guid StateId { get; set; }

    [Required(ErrorMessage = "Country is required!")]
    [Display(Name = "Country")]
    public Guid CountryId { get; set; }

    [Required(ErrorMessage = "Role is required!")]
    [Display(Name = "Role")]
    public Guid RoleId { get; set; }

    public string? Role { get; set; }

    [StringLength(12)]
    public string? Zipcode { get; set; }

    [Required(ErrorMessage = "Phone is required!")]
    [StringLength(10)]
    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Phone Number!")]
    public string Phone { get; set; } = null!;

    [Display(Name = "Status")]
    public bool? IsActive { get; set; }

    public bool IsFirstLogin { get; set; }
}
