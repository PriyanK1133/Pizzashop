using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class ModifierVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required!")]
    [StringLength(20)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required!")]
    [Range(0, 100000,ErrorMessage = "Rate must bet between 0 to 100000!")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required!")]
    [Range(1, 10000,ErrorMessage = "Quantity must be between 1 and 10000!")]
    public int? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required!")]
    [Display(Name = "Unit")]
    public Guid? UnitId { get; set; }

    public string? Description { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }

}
