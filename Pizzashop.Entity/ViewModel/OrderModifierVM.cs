namespace Pizzashop.Entity.ViewModel;

public class OrderModifierVM
{
    public Guid Id { get; set; }

    public Guid ModifierId {get; set;}

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}
