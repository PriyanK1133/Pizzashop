using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class CategoryVM
{
    public Guid Id {get; set;}
    
    [StringLength(20)]
    [Required(ErrorMessage = "Name is required!")]
    public string Name { get; set; } = null!;
    public string? Description {get; set;}
    public Guid CreatedBy {get; set;}
    public Guid UpdatedBy {get; set;}
}
