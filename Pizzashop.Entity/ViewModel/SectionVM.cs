namespace Pizzashop.Entity.ViewModel;

public class SectionVM
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int AssignedTables {get; set;}
    public int AvailableTables {get;set;}
    public int OccupiedTables {get; set;}
    public int WaitingCustomers {get; set;}
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}