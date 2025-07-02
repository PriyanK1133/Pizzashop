namespace Pizzashop.Entity.ViewModel;

public class OrderItemVM
{
    public Guid Id {get; set;}
    public Guid ItemId { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Status { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal TaxPercentage {get; set;}

    public decimal TotalModifierAmount {get; set;}

    public string? Instruction { get; set; }

    public List<OrderModifierVM> OrderModifiers { get; set; } = new();

    public List<Guid> ModifierIds {get; set; } = new();
}
