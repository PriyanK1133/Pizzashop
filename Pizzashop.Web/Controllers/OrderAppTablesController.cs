using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Controllers;

public class OrderAppTablesController : Controller
{
    private readonly IOrderAppService _orderAppService;

    public OrderAppTablesController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }


    #region Tables
    [Route("OrderApp/Tables")]
    public async Task<IActionResult> Tables()
    {
        Response<IEnumerable<SectionVM>> response = await _orderAppService.GetAllSectionsAsync();
        return View(response.Data);
    }

    [Route("OrderApp/GetAllSections")]
    public async Task<JsonResult> GetAllSections()
    {
        Response<IEnumerable<SectionVM>> response = await _orderAppService.GetAllSectionsAsync();
        SelectList sections = new(response.Data, "Id", "Name");
        return Json(new { success = true, sections });
    }

    [Route("OrderApp/GetTableListPartial")]
    public async Task<IActionResult> GetTableListPartial(Guid id)
    {
        Response<IEnumerable<OrderTableVM>> response = await _orderAppService.GetTableDetailsBySectionIdAsync(id);

        if (!response.Success)
        {
            return PartialView("_TableList", new List<OrderTableVM>());
        }

        return PartialView("_TableList", response.Data);
    }

    [Route("OrderApp/GetAvailableTablesBySectionId")]
    public async Task<JsonResult> GetAvailableTablesBySectionId(Guid sectionId)
    {
        Response<IEnumerable<OrderTableVM>> response = await _orderAppService.GetAvailableTablesBySectionIdAsync(sectionId);
        return Json(response);

    }

    [Route("OrderApp/AssignTables")]
    public async Task<JsonResult> AssignTables(CustomerDetailsVM model, List<Guid> tables)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = Constants.InvalidModelMessage });
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;
        model.IsWaiting = false;

        Response<Guid?> response = await _orderAppService.AssignTablesAsync(model, tables);

        return Json(response);
    }

    [Route("OrderApp/AssignTablesToWaitingCustomer")]
    public async Task<JsonResult> AssignTablesToWaitingCustomer(Guid waitingTokenId, Guid sectionId, List<Guid> tableIds)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        Response<Guid?> response = await _orderAppService.AssignTablesToWaitingCustomerAsync(waitingTokenId, sectionId, tableIds, updatorId);
        return Json(response);
    }

    #endregion Tables
}
