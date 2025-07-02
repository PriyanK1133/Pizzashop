namespace Pizzashop.Entity.ViewModel;

public class MenuVM
{
    public IList<CategoryVM> Categories = new List<CategoryVM>();
    public IList<ItemListVM> ItemList = new List<ItemListVM>();
    public IList<ModifierGroupVM> ModifierGroups  = new List<ModifierGroupVM>();
    public IList<ModifierVM> Modifiers = new List<ModifierVM>();
}
