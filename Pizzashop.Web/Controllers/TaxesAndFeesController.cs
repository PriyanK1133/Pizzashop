using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Controllers;

[PermissionAuthorize(Constants.CanView)]
public class TaxesAndFeesController : Controller
{
    private readonly ITaxAndFeeService _taxAndFeeService;
    public TaxesAndFeesController(ITaxAndFeeService taxAndFeeService)
    {
        _taxAndFeeService = taxAndFeeService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetPagedList(string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        Response<PagedResult<TaxAndFeeVM>> response = await _taxAndFeeService.GetPagedListAsync(searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return PartialView("_TaxesAndFees", new PagedResult<TaxAndFeeVM>());
        }

        return PartialView("_TaxesAndFees", response.Data!);
    }

    [PermissionAuthorize(Constants.CanEdit)]

    public async Task<IActionResult> Add(TaxAndFeeVM model)
    {

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<TaxAndFeeVM?> response = await _taxAndFeeService.AddAsync(model);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });

    }

    public async Task<IActionResult> GetById(Guid id)
    {
        Response<TaxAndFeeVM?> response = await _taxAndFeeService.GetById(id);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return PartialView("_AddTax", response.Data!);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> Edit(TaxAndFeeVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddTax", model);
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        model.UpdatedBy = updatorId;

        Response<TaxAndFeeVM?> response = await _taxAndFeeService.EditAsync(model);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }
    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {

        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _taxAndFeeService.DeleteAsync(id, deletorId);

        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

}
