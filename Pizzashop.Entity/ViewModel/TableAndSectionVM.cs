namespace Pizzashop.Entity.ViewModel;

public class TableAndSectionVM
{
    public Guid SectionId {get; set;}

    public string SectionName {get; set;} = null!;

    public PagedResult<TableVM> TablePagination {get; set;} = new();
}
