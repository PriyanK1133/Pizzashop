using System.ComponentModel.DataAnnotations;

namespace Pizzashop.Entity.ViewModel;

public class CustomerHistoryVM
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal AverageBill { get; set; }

    public string MobileNumber { get; set; } = null!;

    [Display(Name = "Coming Since")]
    public DateTime ComingSince { get; set; }

    public decimal MaxOrder { get; set; }

    public int Visits { get; set; }

    public List<CustomerOrder> CustomerOrders { get; set; } = new();
}
public class CustomerOrder
{
    public Guid Id { get; set; }

    public DateOnly OrderDate { get; set; }

    public string OrderType { get; set; } = null!;

    public string Payment { get; set; } = null!;

    public int Items { get; set; }

    public decimal Amount { get; set; }
}