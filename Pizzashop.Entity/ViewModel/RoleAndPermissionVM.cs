namespace Pizzashop.Entity.ViewModel;

public class RoleAndPermissionVM
{
    public Guid RoleId {get; set;} 

    public List<PermissionVM> Permissions {get; set;} = new();

}
