using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Web.Controllers;

[Authorize(Roles = Constants.AccountManager + "," + Constants.Chef)]
public class KOTController : Controller
{
    private readonly IOrderAppService _orderAppService;
    private readonly IMenuService _menuService;

    public KOTController(IOrderAppService orderAppService, IMenuService menuService)
    {
        _orderAppService = orderAppService;
        _menuService = menuService;
    }

    #region KOT
    [Route("OrderApp/KOT")]
    public async Task<IActionResult> KOT()
    {
        Response<IEnumerable<CategoryVM>> response = await _menuService.GetAllCategoriesAsync();

        return View(response.Data);
    }

    [Route("OrderApp/GetKOTOrders")]
    public async Task<IActionResult> GetKOTOrders(Guid? categoryId, string status = "")
    {
        Response<IEnumerable<KOTOrderVM>> response = await _orderAppService.GetKOTOrdersAsync(status, categoryId.GetValueOrDefault());

        return PartialView("_KOTList", response.Data);
    }

    [Route("OrderApp/UpdateOrderItemsStatus")]
    public async Task<JsonResult> UpdateOrderItemsStatus(Guid orderId, string itemStatus, List<OrderItemVM> orderItems)
    {
        Response<bool> response = await _orderAppService.UpdateOrderItemStatusAsync(orderId, itemStatus, orderItems);
        return Json(new { success = response.Success, message = response.Message });
    }

    #endregion KOT

}
