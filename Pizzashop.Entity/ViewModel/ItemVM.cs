using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class ItemVM
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Category is required!")]
    [Display(Name = "Category")]
    public Guid? CategoryId { get; set; }

    [StringLength(20)]
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; } = null!;

    [StringLength(20)]
    [Display(Name = "Type")]
    [Required(ErrorMessage = "Type is required!")]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required!")]
    [DataType(DataType.Currency, ErrorMessage = "Rate must be a number!")]
    [Range(0, 100000, ErrorMessage = "Rate must be between 0 to 100000!")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required!")]
    [DataType(DataType.Currency, ErrorMessage = "Quantity must be a number!")]
    [Range(1, 10000,ErrorMessage = "Quantity must be between 1 to 10000!")]
    public int? Quantity { get; set; }

    [Display(Name = "Unit")]
    [Required(ErrorMessage = "Unit is required!")]
    public Guid? UnitId { get; set; }

    [StringLength(20)]
    public string? Shortcode { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    [Display(Name = "Default Tax")]
    [Required(ErrorMessage = "Default Tax is required!")]
    public bool IsDefaultTax { get; set; } = true;

    [Required(ErrorMessage = "Tax Percentage is required!")]
    [Range(0, 99.99,ErrorMessage = "Tax Percentage must be between 0 to 99.99!")]
    public decimal? TaxPercentage { get; set; }

    public bool IsFavourite { get; set; }

    [Display(Name = "Available")]
    public bool IsAvailable { get; set; } = true;

    public List<ModifierGroupForItemVM> SelectedModifierGroups { get; set; } = new();

    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }
}
