namespace Pizzashop.Entity.ViewModel;

public class OrderListVM
{
    public Guid Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string PaymentMode { get; set; } = null!;

    public int Rating { get; set; }

    public decimal OrderTotal { get; set; } 
}
