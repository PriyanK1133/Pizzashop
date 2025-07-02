namespace Pizzashop.Entity.DTO;

public class ItemSummaryDTO
{
    public Guid Id {get; set;}
    public string Name {get; set;} = null!;
    public string? Image {get; set;}
    public int OrderCount {get; set;}
}
