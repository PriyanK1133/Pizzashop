namespace Pizzashop.Entity.ViewModel;

public class GraphDataVM
{
    public List<GraphPointVM> RevenueData {get; set;} = new();
    public List<GraphPointVM> CustomerGrowthData {get; set;} = new();
}

public class GraphPointVM{
    public string Date {get; set;} = null!;
    public decimal Value {get; set;}
}