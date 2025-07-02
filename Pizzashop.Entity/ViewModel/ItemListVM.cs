using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class ItemListVM
{
    public Guid Id {get; set;}
    
    [StringLength(20)]
    public string Name {get; set;} = null!;
    
    public string Type { get; set; } = null!;

    public decimal Rate {get; set;}

    public int Quantity { get; set; }

    public decimal TaxPercentage {get; set;}

     public string? Image { get; set; }

    public bool IsAvailable {get; set;}
    public bool IsFavourite {get; set;}
}
