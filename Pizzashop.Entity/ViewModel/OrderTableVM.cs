
namespace Pizzashop.Entity.ViewModel;
using Pizzashop.Entity.Constants;

public class OrderTableVM
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public short Capacity { get; set; }

    public string Status { get; set; } = Constants.OrderTableAvailable;

    public Guid? OrderId {get; set;}

    public DateTime? OrderTime { get; set; }

    public decimal? OrderTotal { get; set; }
}
