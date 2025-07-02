using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class ModifierGroupVM
{
    public Guid Id {get; set;}

    [StringLength(20)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid CreatedBy {get; set;}

    public Guid UpdatedBy {get; set;}
}
