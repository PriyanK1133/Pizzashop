namespace Pizzashop.Entity.ViewModel;

public class DashboardVM
{
    public decimal TotalSales {get; set;}
    public int TotalOrder {get; set;}
    public decimal AvgOrderValue {get; set;}
    public TimeSpan AvgWaitingTime {get; set;}
    public List<ItemSummaryVM> TopSellingItems {get; set;} = new();
    public List<ItemSummaryVM> LeastSellingItems {get; set;} = new();
    public int WaitingListCount {get; set;}
    public int NewCustomerCount {get; set;}
}

public class ItemSummaryVM {
    public Guid Id {get; set;}
    public string Name {get; set;} = null!;
    public string? Image {get; set;}
    public int OrderCount {get; set;}
}
