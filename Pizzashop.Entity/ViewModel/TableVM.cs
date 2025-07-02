using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class TableVM
{
    public Guid Id { get; set; }

    [StringLength(20)]
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Capacity is required!")]
    [Range(1,100, ErrorMessage = "Capacity must be between 1 to 100!")]
    public short? Capacity { get; set; }

    [Display(Name = "Status*")]
    public bool IsOccupied { get; set; }

    [Display(Name = "Sections*")]
    public Guid SectionId { get; set; }

    public string? SectionName { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }
}
