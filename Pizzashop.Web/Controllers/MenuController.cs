using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[PermissionAuthorize(Constants.CanView)]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region  Categories
    public async Task<IActionResult> GetCategories()
    {
        Response<IEnumerable<CategoryVM>> response = await _menuService.GetAllCategoriesAsync();
        if (!response.Success)
        {
            return PartialView("_Category");
        }
        IEnumerable<CategoryVM> model = response.Data!;
        return PartialView("_Category", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> AddCategory(CategoryVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<CategoryVM?> response = await _menuService.AddCategoryAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> EditCategory(CategoryVM model)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.UpdatedBy = updatorId;

        Response<CategoryVM?> response = await _menuService.EditCategoryAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteCategory(Guid id)
    {

        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        Response<bool> response = await _menuService.DeleteCategoryAsync(id, deletorId);
        return Json(response);
    }

    public async Task<IActionResult> ChangeCategoriesOrder(List<Guid> sortedIds)
    {
        Response<bool> response = await _menuService.ChangeCategoriesOrder(sortedIds);

        if (!response.Success)
        {
            return Json(new { success = false });
        }

        return Json(new { success = false });
    }

    #endregion

    #region  Items

    public async Task<IActionResult> GetItemsByCategory(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            Response<IEnumerable<CategoryVM>> categoriesResponse = await _menuService.GetAllCategoriesAsync();
            if (!categoriesResponse.Success)
            {
                TempData["error"] = "No categories found!";
                return PartialView("_Items");
            }
            id = categoriesResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<IEnumerable<ItemListVM>> response = await _menuService.GetItemsByCategoryIdAsync(id);
        if (!response.Success)
        {
            return PartialView("_Items");
        }
        IEnumerable<ItemListVM> model = response.Data!;
        return PartialView("_Items", model);
    }

    public async Task<IActionResult> GetPagedItems(Guid categoryId, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        if (categoryId.Equals(Guid.Empty))
        {
            Response<IEnumerable<CategoryVM>> categoriesResponse = await _menuService.GetAllCategoriesAsync();
            if (!categoriesResponse.Success)
            {
                TempData["error"] = "No categories found!";
                return PartialView("_Items");
            }
            categoryId = categoriesResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<PagedResult<ItemListVM>> response = await _menuService.GetPagedItemsAsync(categoryId, searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_Items");
        }

        PagedResult<ItemListVM> model = response.Data!;
        return PartialView("_Items", model);
    }

    public async Task<IActionResult> GetItemById(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            return PartialView("_AddItemModal");
        }

        Response<ItemVM?> response = await _menuService.GetItemByIdAsync(id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return PartialView("_AddItemModal");
        }
        ItemVM model = response.Data!;

        return PartialView("_AddItemModal", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddItem(ItemVM model, IFormFile? imageFile)
    {
        ModelState.Remove("Id");
        if (!ModelState.IsValid)
        {
            return PartialView("_AddItemModal", model);
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        if (imageFile == null)
        {
            FileUploadHandler.DeleteFile(model.Image);
            model.Image = null;
        }
        else
        {
            string itemImage = await FileUploadHandler.UploadImage(imageFile);
            if (!string.IsNullOrEmpty(itemImage))
            {
                FileUploadHandler.DeleteFile(model.Image);
                model.Image = itemImage;
            }
        }

        Response<ItemVM?> response = await _menuService.AddItemAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditItem(ItemVM model, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddItemModal", model);
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.UpdatedBy = updatorId;

        if (imageFile != null)
        {
            string itemImage = await FileUploadHandler.UploadImage(imageFile);
            if (!string.IsNullOrEmpty(itemImage))
            {
                FileUploadHandler.DeleteFile(model.Image);
                model.Image = itemImage;
            }
        }
        else if (string.IsNullOrEmpty(model.Image))
        {
            FileUploadHandler.DeleteFile(model.Image);
            model.Image = null;
        }

        Response<ItemVM?> response = await _menuService.EditItemAsync(model);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteItem(Guid id)
    {
        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _menuService.DeleteItemAsync(id, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteManyItem(IEnumerable<Guid> ids)
    {
        if (!ids.Any())
        {
            return Json(new { success = false, message = "No Items Selected to Delete" });
        }

        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _menuService.DeleteManyItemAsync(ids, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    #endregion

    #region ModifierGroups

    public async Task<IActionResult> GetModifierGroups()
    {
        Response<IEnumerable<ModifierGroupVM>> response = await _menuService.GetAllModifierGroupsAsync();
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }
        IEnumerable<ModifierGroupVM> model = response.Data!;
        return PartialView("_ModifierGroup", model);
    }

    public async Task<JsonResult> GetModifierGroupsForModifier(Guid id)
    {
        Response<IEnumerable<ModifierGroupVM>> response = await _menuService.GetModifierGroupsByModifierIdAsync(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        IEnumerable<ModifierGroupVM> data = response.Data!;
        return Json(new { success = true, data });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddModifierGroup(ModifierGroupVM model, List<Guid> modifierIds)
    {

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<ModifierGroupVM?> response = await _menuService.AddModifierGroupAsync(model, modifierIds);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditModifierGroup(ModifierGroupVM model, List<Guid> modifierIds)
    {

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.UpdatedBy = creatorId;

        Response<ModifierGroupVM?> response = await _menuService.EditModifierGroupAsync(model, modifierIds);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message, data = model });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteModifierGroup(Guid id)
    {
        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        Response<bool> response = await _menuService.DeleteModifierGroupAsync(id, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    public async Task<IActionResult> ChangeModifierGroupsOrder(List<Guid> sortedIds)
    {
        Response<bool> response = await _menuService.ChangeModifierGroupsOrder(sortedIds);

        if (!response.Success)
        {
            return Json(new { success = false });
        }

        return Json(new { success = false });
    }

    #endregion

    #region  Modifiers

    public async Task<IActionResult> GetModifierById(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            return PartialView("_AddEditModifierModal");
        }

        Response<ModifierVM?> response = await _menuService.GetModifierByIdAsync(id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return PartialView("_AddEditModifierModal");
        }
        ModifierVM model = response.Data!;

        return PartialView("_AddEditModifierModal", model);
    }

    public async Task<IActionResult> GetModifiersByModifierGroup(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            Response<IEnumerable<ModifierGroupVM>> modifierGroupResponse = await _menuService.GetAllModifierGroupsAsync();
            if (!modifierGroupResponse.Success)
            {
                TempData["error"] = "No Modifier group found!";
                return PartialView("_Modifier");
            }
            id = modifierGroupResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<IEnumerable<ModifierListVM>> response = await _menuService.GetModifiersByGroupIdAsync(id);
        if (!response.Success)
        {
            return PartialView("_Modifier");
        }
        IEnumerable<ModifierListVM> model = response.Data!;
        return PartialView("_Modifier", model);
    }

    public async Task<IActionResult> GetPagedModifiers(Guid modifierGroupId, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        if (modifierGroupId.Equals(Guid.Empty))
        {
            Response<IEnumerable<ModifierGroupVM>> modifierGroupResponse = await _menuService.GetAllModifierGroupsAsync();
            if (!modifierGroupResponse.Success)
            {
                TempData["error"] = "No Modifier Group found!";
                return PartialView("_Modifier");
            }
            modifierGroupId = modifierGroupResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<PagedResult<ModifierListVM>> response = await _menuService.GetPagedModifiersAsync(modifierGroupId, searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_Modifier");
        }

        PagedResult<ModifierListVM> model = response.Data!;
        return PartialView("_Modifier", model);
    }

    public async Task<IActionResult> GetAllModifiers()
    {
        Response<IEnumerable<ModifierListVM>> response = await _menuService.GetAllModifiersAsync();
        if (!response.Success)
        {
            return PartialView("_SelectModifierModal");
        }
        IEnumerable<ModifierListVM> model = response.Data!;
        return PartialView("_SelectModifierModal", model);
    }

    public async Task<IActionResult> GetAllPagedModifiers(string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        Response<PagedResult<ModifierListVM>> response = await _menuService.GetAllPagedModifiersAsync(searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_SelectModifierModal");
        }
        PagedResult<ModifierListVM> model = response.Data!;
        return PartialView("_SelectModifierModal", model);
    }

    public async Task<JsonResult> GetModifierDetailsByModifierGroup(Guid id)
    {
        Response<IEnumerable<ModifierListVM>> response = await _menuService.GetModifiersByGroupIdAsync(id);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }
        IEnumerable<ModifierListVM> data = response.Data!;
        return Json(new { success = true, data });
    }

    public async Task<JsonResult> AddModifier(ModifierVM model, List<Guid> modifierGroups)
    {

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<ModifierVM?> response = await _menuService.AddModifierAsync(model, modifierGroups);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    public async Task<JsonResult> EditModifier(ModifierVM model, List<Guid> modifierGroups)
    {
        ModelState.Remove("model.Id");
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Incorrect Details!" });
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.UpdatedBy = updatorId;

        Response<ModifierVM?> response = await _menuService.EditModifierAsync(model, modifierGroups);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    public async Task<JsonResult> DeleteModifier(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            return Json(new { success = false, message = "Modifier not found!" });
        }

        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        Response<bool> response = await _menuService.DeleteModifierAsync(id, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    public async Task<JsonResult> DeleteManyModifier(IEnumerable<Guid> ids)
    {
        if (!ids.Any())
        {
            return Json(new { success = false, message = "No Items Selected to Delete" });
        }

        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _menuService.DeleteManyModifierAsync(ids, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    #endregion

    #region BindData
    public async Task<JsonResult> BindData()
    {
        SelectList? categories = null;
        SelectList? units = null;
        SelectList? modifier_groups = null;

        Response<IEnumerable<CategoryVM>> categoryResponse = await _menuService.GetAllCategoriesAsync();
        if (categoryResponse.Success)
        {
            categories = new(categoryResponse.Data, "Id", "Name");
        }

        Response<IEnumerable<UnitVM>> unitResponse = await _menuService.GetAllUnitsAsync();
        if (unitResponse.Success)
        {
            units = new(unitResponse.Data, "Id", "Name");
        }

        Response<IEnumerable<ModifierGroupVM>> modifierGroupResponse = await _menuService.GetAllModifierGroupsAsync();
        if (modifierGroupResponse.Success)
        {
            modifier_groups = new(modifierGroupResponse.Data, "Id", "Name");
        }

        return Json(new { categories, units, modifier_groups });
    }

    #endregion

}
