using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class TaxAndFeeVM
{
    public Guid Id { get; set; }

    [StringLength(20)]
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; } = null!;
    
    [Required(ErrorMessage = "Type is required!")]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = "Tax Value is required!")]
    [Range(0, 100000, ErrorMessage = "Tax Value must be between 0 to 100000")]
    public decimal? TaxAmount { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDefault { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }
}
