namespace Pizzashop.Entity.ViewModel;

public class PermissionVM
{
    public Guid Id {get; set;}

    public string Name {get; set;} = null!;

    public bool CanView {get; set;}
    public bool CanEdit {get; set;}
    public bool CanDelete {get; set;}
}
