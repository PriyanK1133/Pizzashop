namespace Pizzashop.Entity.ViewModel;

public class CustomerVM
{
    public Guid Id {get; set;}

    public string Name {get; set;} = null!;

    public string Email {get; set;} = null!;

    public string Phone {get; set;} = null!;

    public DateOnly Date {get; set;} 

    public int TotalOrder {get; set;}
}
