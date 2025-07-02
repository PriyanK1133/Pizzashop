using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class CustomerDetailsVM
{
    public Guid Id { get; set; }

    [StringLength(20)]
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    [Required(ErrorMessage = "Email is Required!")]
    [EmailAddress(ErrorMessage = "Invalid Email Address!")]
    public string Email { get; set; } = null!;

    [StringLength(10)]
    [Required(ErrorMessage = "Mobile Number is required!")]
    [RegularExpression("[0-9]{10}", ErrorMessage = "Invalid Mobile Number!")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "No. of Person(s) is required!")]
    [Range(1, 100, ErrorMessage = "No. of Person(s) must be between 1 and 100")]
    public int? NumberOfPerson { get; set; }

    [Required(ErrorMessage = "Section is required!")]
    public Guid SectionId { get; set; }

    public int TableCapacity { get; set; }

    public Guid CustomerId { get; set; }

    public bool IsWaiting { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid UpdatedBy { get; set; }

    public List<Guid> Tables { get; set; } = new();
}
