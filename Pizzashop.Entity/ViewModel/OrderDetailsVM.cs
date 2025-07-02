namespace Pizzashop.Entity.ViewModel;

public class OrderDetailsVM
{
    public Guid Id { get; set; }

    public string? InvoiceNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime PlacedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public TimeSpan OrderDuration { get; set; }

    public CustomerDetailsVM CustomerDetails { get; set; } = new();

    public List<string> TableNames { get; set; } = new();

    public string SectionName { get; set; } = null!;

    public List<OrderItemVM> OrderItems { get; set; } = new();

    public decimal SubTotal { get; set; }

    public List<OrderTaxVM> OrderTaxes { get; set; } = new();

    public List<Guid> OrderTaxIds { get; set; } = new();

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? Comment { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid UpdatedBy { get; set; }
}
