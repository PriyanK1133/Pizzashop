namespace Pizzashop.Entity.ViewModel;

public class OrderTaxVM
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type {get; set;} = null!;

    public decimal Rate {get; set;}

    public decimal TotalTax { get; set; }
}