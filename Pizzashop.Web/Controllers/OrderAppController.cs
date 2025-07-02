using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Utils;
using Rotativa.AspNetCore;

namespace Pizzashop.Web.Controllers;

[Authorize(Roles = Constants.AccountManager)]
public class OrderAppController : Controller
{
    private readonly IOrderAppService _orderAppService;
    private readonly IMenuService _menuService;
    private readonly IOrderService _orderService;
        private readonly IUserService _userService;

    public OrderAppController(IOrderAppService orderAppService, IMenuService menuService, IOrderService orderService, IUserService userService)
    {
        _orderAppService = orderAppService;
        _menuService = menuService;
        _orderService = orderService;
        _userService = userService;
    }
    #region WaitingList

    public IActionResult GetWaitingTokenForm()
    {
        return PartialView("_CustomerDetails", new CustomerDetailsVM());
    }

    public async Task<IActionResult> WaitingList()
    {
        Response<IEnumerable<SectionVM>> response = await _orderAppService.GetAllSectionsAsync();
        return View(response.Data);
    }

    public async Task<IActionResult> GetWaitingListPartial(Guid sectionId)
    {
        Response<IEnumerable<CustomerDetailsVM>> response = await _orderAppService.GetWaitingListAsync(sectionId);

        return PartialView("_WaitingListPartial", response.Data);
    }

    public async Task<JsonResult> GetWaitingList(Guid sectionId)
    {
        Response<IEnumerable<CustomerDetailsVM>> response = await _orderAppService.GetWaitingListAsync(sectionId);

        return Json(response);
    }

    public async Task<JsonResult> AddWaitingToken(CustomerDetailsVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = Constants.InvalidModelMessage });
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;
        model.IsWaiting = true;

        Response<CustomerDetailsVM?> response = await _orderAppService.AddCustomerDetailsAsync(model);

        return Json(response);
    }

    public async Task<JsonResult> EditWaitingToken(CustomerDetailsVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = Constants.InvalidModelMessage });
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.UpdatedBy = updatorId;
        model.IsWaiting = true;

        Response<CustomerDetailsVM?> response = await _orderAppService.UpdateCustomerDetailsAsync(model);

        return Json(response);
    }

    public async Task<JsonResult> GetCustomerDetailsByEmail(string email)
    {
        Response<CustomerDetailsVM?> response = await _orderAppService.GetCustomerDetailsByEmailAsync(email);

        return Json(response);
    }

    public async Task<JsonResult> DeleteWaitingToken(Guid id)
    {
        Response<bool> response = await _orderAppService.DeleteWaitingToken(id);
        return Json(response);
    }

    #endregion WaitingList

    #region Menu

    public async Task<IActionResult> GetAllCategories()
    {
        Response<IEnumerable<CategoryVM>> response = await _menuService.GetAllCategoriesAsync();
        return Json(response);
    }

    public async Task<IActionResult> Menu(Guid id)
    {
        Response<OrderDetailsVM?> response = await _orderService.GetByIdAsync(id);

        OrderDetailsVM? model = response.Data;
        if (model == null || (model.Status != Constants.OrderPending && model.Status != Constants.OrderInProgress && model.Status != Constants.OrderServed))
        {
            return View();
        }
        return View(response.Data);
    }

    public async Task<IActionResult> GetItemsByCategoryId(Guid categoryId)
    {
        Response<IEnumerable<ItemListVM>> response = await _menuService.GetAvailableItemsByCategoryIdAsync(categoryId);
        return PartialView("_MenuItems", response.Data);
    }

    public async Task<IActionResult> GetFavoriteItems()
    {
        Response<IEnumerable<ItemListVM>> response = await _menuService.GetFavoriteItemsAsync();
        return PartialView("_MenuItems", response.Data);
    }

    public async Task<JsonResult> ToggleFavoriteItem(Guid itemId)
    {
        Response<bool> response = await _menuService.ToggleFavoriteItemAsync(itemId);
        return Json(response);
    }

    public async Task<IActionResult> GetModifierGroupsForItem(Guid itemId)
    {
        Response<IEnumerable<ModifierGroupForItemVM>> response = await _menuService.GetApplicableModifiersForItem(itemId);
        return PartialView("_AddModifiersModal", response.Data);
    }

    public async Task<JsonResult> EditCustomerDetails(CustomerDetailsVM model)
    {
        if (model.TableCapacity < model.NumberOfPerson)
        {
            return Json(new { success = false, message = MessageConstants.ExceedTableCapacityMessage});
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.UpdatedBy = updatorId;

        Response<CustomerDetailsVM?> response = await _orderAppService.UpdateCustomerDetailsAsync(model);
        return Json(response);
    }

    public async Task<JsonResult> EditOrderComment(Guid id, string? comment)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<string?> response = await _orderAppService.EditOrderCommentAsync(id, comment, updatorId);

        return Json(response);
    }

    public async Task<JsonResult> SaveOrderItemInstruction(Guid id, string? specialInstruction)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<string?> response = await _orderAppService.SaveOrderItemInstructionAsync(id, specialInstruction, updatorId);

        return Json(response);
    }
    public async Task<JsonResult> GetOrderItemInstruction(Guid id)
    {
        return Json(await _orderAppService.GetOrderItemInstructionAsync(id));
    }

    public IActionResult GetCustomerDetailsPartial(CustomerDetailsVM model)
    {
        return PartialView("_MenuCustomerDetails", model);
    }

    public async Task<JsonResult> SaveOrder(OrderDetailsVM model)
    {
        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<bool> response = await _orderAppService.SaveOrderAsync(model);
        return Json(response);
    }

    public async Task<JsonResult> IsItemQuantityPrepared(Guid orderItemId, int quantity)
    {
        Response<bool> response = await _orderAppService.IsItemQuantityPrepared(orderItemId, quantity);
        return Json(response);
    }

    public async Task<JsonResult> IsOrderServed(Guid orderId)
    {
        Response<bool> response = await _orderAppService.IsOrderServedAsync(orderId);
        return Json(response);
    }

    public async Task<JsonResult> CompleteOrder(Guid orderId, string paymentMethod)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _orderAppService.CompleteOrderAsync(orderId, paymentMethod, updatorId);
        return Json(response);
    }

    public async Task<IActionResult> SaveRating(RatingVM model)
    {
        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<bool> response = await _orderAppService.SaveRatingAsync(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
        }
        else
        {
            TempData["success"] = response.Message;
        }

        return RedirectToAction("Tables");
    }

    public async Task<IActionResult> CancelOrder(Guid id)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _orderAppService.CancelOrderAsync(id, updatorId);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Menu", new { id });
        }

        TempData["success"] = response.Message;
        return RedirectToAction("Tables");
    }

    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        Response<OrderDetailsVM?> response = await _orderService.GetByIdAsync(id);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Menu");
        }
        return new ViewAsPdf("../Orders/OrderPdf", response.Data)
        {
            FileName = "Invoice.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(0, 0, 0, 0),
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
        };

    }
    #endregion Menu


    public async Task<IActionResult> Profile()
    {
        Guid currentUser = (Guid)SessionUtils.GetUserId(HttpContext)!;
        Response<UserVM?> response = await _userService.GetByIdAsync(currentUser);

        if (!response.Success)
        {
            return RedirectToAction("Index");
        }

        UserVM model = response.Data!;

        return View("~/Views/Home/Profile.cshtml", model);
    }

}
