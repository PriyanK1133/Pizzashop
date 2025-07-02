using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IMenuService
{
    #region Category
    Task<Response<CategoryVM?>> GetCategoryByIdAsync(Guid id);
    Task<Response<IEnumerable<CategoryVM>>> GetAllCategoriesAsync();
    Task<Response<CategoryVM?>> AddCategoryAsync(CategoryVM model);
    Task<Response<CategoryVM?>> EditCategoryAsync(CategoryVM model);
    Task<Response<bool>> DeleteCategoryAsync(Guid id, Guid deltorId);
    Task<Response<bool>> ChangeCategoriesOrder(List<Guid> sortedIds);
    #endregion Category

    #region Items
    Task<Response<IEnumerable<ItemListVM>>> GetItemsByCategoryIdAsync(Guid categoryId);
    Task<Response<IEnumerable<ItemListVM>>> GetAvailableItemsByCategoryIdAsync(Guid categoryId);
    Task<Response<IEnumerable<ItemListVM>>> GetFavoriteItemsAsync();
    Task<Response<PagedResult<ItemListVM>>> GetPagedItemsAsync(Guid categoryId, string searchString, int page, int pagesize, bool isASC);
    Task<Response<IEnumerable<ModifierGroupForItemVM>>> GetApplicableModifiersForItem(Guid itemId);
    Task<Response<ItemVM?>> GetItemByIdAsync(Guid id);
    Task<Response<ItemVM?>> AddItemAsync(ItemVM model);
    Task<Response<ItemVM?>> EditItemAsync(ItemVM model);
    Task<Response<bool>> DeleteItemAsync(Guid id, Guid deletorId);
    Task<Response<bool>> DeleteManyItemAsync(IEnumerable<Guid> ids, Guid deletorId);
    Task<Response<bool>> ToggleFavoriteItemAsync(Guid itemId);
    #endregion Items

    #region Modifier Groups
    Task<Response<IEnumerable<ModifierGroupVM>>> GetAllModifierGroupsAsync();
    Task<Response<ModifierGroupVM?>> GetModifierGroupByIdAsync(Guid id);
    Task<Response<PagedResult<ModifierListVM>>> GetPagedModifiersAsync(Guid modifierGroupId, string searchString, int page, int pagesize, bool isASC);
    Task<Response<IEnumerable<ModifierGroupVM>>> GetModifierGroupsByModifierIdAsync(Guid modifierId);
    Task<Response<ModifierGroupVM?>> AddModifierGroupAsync(ModifierGroupVM model, List<Guid> modifierIds);
    Task<Response<ModifierGroupVM?>> EditModifierGroupAsync(ModifierGroupVM model, List<Guid> modifierIds);
    Task<Response<bool>> DeleteModifierGroupAsync(Guid id, Guid deletorId);
    Task<Response<bool>> ChangeModifierGroupsOrder(List<Guid> sortedIds);
    #endregion Modifier Groups

    #region Modifiers
    Task<Response<IEnumerable<ModifierListVM>>> GetModifiersByGroupIdAsync(Guid modifierGroupId);
    Task<Response<IEnumerable<ModifierListVM>>> GetAllModifiersAsync();
    Task<Response<PagedResult<ModifierListVM>>> GetAllPagedModifiersAsync(string searchString, int page, int pagesize, bool isASC);
    Task<Response<ModifierVM?>> GetModifierByIdAsync(Guid id);
    Task<Response<ModifierVM?>> AddModifierAsync(ModifierVM model, List<Guid> modifierGroups);
    Task<Response<ModifierVM?>> EditModifierAsync(ModifierVM model, List<Guid> modifierGroups);
    Task<Response<bool>> DeleteModifierAsync(Guid id, Guid deletorId);
    Task<Response<bool>> DeleteManyModifierAsync(IEnumerable<Guid> ids, Guid deletorId);
    #endregion Modifiers

    #region Units
    Task<Response<IEnumerable<UnitVM>>> GetAllUnitsAsync();
    #endregion Units

}
