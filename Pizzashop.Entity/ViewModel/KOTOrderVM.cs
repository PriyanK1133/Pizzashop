namespace Pizzashop.Entity.ViewModel;

public class KOTOrderVM
{
    public Guid Id { get; set; }

    public string? ItemStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> TableNames { get; set; } = new();

    public string SectionName { get; set; } = null!;

    public List<OrderItemVM> OrderItems { get; set; } = new();

    public string? OrderInstruction { get; set; }
}

