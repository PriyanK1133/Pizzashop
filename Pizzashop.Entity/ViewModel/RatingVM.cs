namespace Pizzashop.Entity.ViewModel;

public class RatingVM
{
    public Guid Id {get; set;}
    public Guid OrderId {get; set;}
    public int FoodRating {get; set;} 
    public int ServiceRating {get; set;}
    public int AmbienceRating {get; set;}
    public string? Comment {get; set;}
    public Guid CreatedBy {get; set;}
    public Guid UpdatedBy {get; set;}
}
