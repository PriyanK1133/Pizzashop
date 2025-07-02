using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Controllers;

[PermissionAuthorize(Constants.CanView)]
public class TableAndSectionController : Controller
{

    private readonly ITableAndSectionService _tableAndSectionService;

    public TableAndSectionController(ITableAndSectionService tableAndSectionService)
    {
        _tableAndSectionService = tableAndSectionService;
    }

    #region Sections
    public async Task<IActionResult> Index()
    {

        Response<IEnumerable<SectionVM>> response = await _tableAndSectionService.GetAllSectionsAsync();
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return View();
        }

        return View(response.Data);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddSection(SectionVM model)
    {
        if (!ModelState.IsValid)
        {
            TempData["error"] = "Invalid Details";
            return RedirectToAction("Index");
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<SectionVM?> response = await _tableAndSectionService.AddSectionAsync(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
        }
        else
        {
            TempData["success"] = response.Message;
        }

        return RedirectToAction("Index");

    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditSection(SectionVM model)
    {
        if (!ModelState.IsValid)
        {
            TempData["error"] = "Invalid Details";
            return RedirectToAction("Index");
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.UpdatedBy = updatorId;

        Response<SectionVM?> response = await _tableAndSectionService.EditSectionAsync(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
        }
        else
        {
            TempData["success"] = response.Message;
        }

        return RedirectToAction("Index");
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<IActionResult> DeleteSection(Guid id)
    {
        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        Response<bool> response = await _tableAndSectionService.DeleteSectionAsync(id, deletorId);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }
        TempData["success"] = response.Message;

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ChangeSectionOrder(List<Guid> sortedIds)
    {
        Response<bool> response = await _tableAndSectionService.ChangeSectionOrder(sortedIds);

        if (!response.Success)
        {
            return Json(new { success = false });
        }

        return Json(new { success = false });
    }
    #endregion Sections

    #region  Tables
    public async Task<IActionResult> GetTableById(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            return PartialView("_AddEditTableModal", new TableVM());
        }

        Response<TableVM?> response = await _tableAndSectionService.GetTableByIdAsync(id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        TableVM model = response.Data!;
        return PartialView("_AddEditTableModal", model);

    }

    public async Task<IActionResult> GetTablesBySection(Guid id)
    {
        if (id.Equals(Guid.Empty))
        {
            Response<IEnumerable<SectionVM>> sectionResponse = await _tableAndSectionService.GetAllSectionsAsync();
            if (!sectionResponse.Success)
            {
                TempData["error"] = "No Sections found!";
                return PartialView("_Tables");
            }
            id = sectionResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<IEnumerable<TableVM>> response = await _tableAndSectionService.GetTablesBySectionIdAsync(id);
        if (!response.Success)
        {
            return PartialView("_Tables");
        }
        IEnumerable<TableVM> model = response.Data!;
        return PartialView("_Tables", model);
    }

    public async Task<IActionResult> GetPagedTables(Guid sectionId, string searchString = "", int page = 1, int pagesize = 5, bool isASC = true)
    {
        if (sectionId.Equals(Guid.Empty))
        {
            Response<IEnumerable<SectionVM>> sectionResponse = await _tableAndSectionService.GetAllSectionsAsync();
            if (!sectionResponse.Success)
            {
                TempData["error"] = "No Sections found!";
                return RedirectToAction("Index");
            }
            sectionId = sectionResponse.Data!.FirstOrDefault()!.Id;
        }

        Response<TableAndSectionVM?> response = await _tableAndSectionService.GetPagedTablesAsync(sectionId, searchString, page, pagesize, isASC);
        if (!response.Success)
        {
            return RedirectToAction("Index");
        }

        TableAndSectionVM model = response.Data!;
        return PartialView("_Tables", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> AddTable(TableVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddEditTableModal", model);
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.CreatedBy = creatorId;
        model.UpdatedBy = creatorId;

        Response<TableVM?> response = await _tableAndSectionService.AddTableAsync(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> EditTable(TableVM model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_AddEditTableModal", model);
        }

        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.UpdatedBy = updatorId;

        Response<TableVM?> response = await _tableAndSectionService.EditTableAsync(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteTable(Guid id)
    {
        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _tableAndSectionService.DeleteTableAsync(id, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<JsonResult> DeleteManyTables(IEnumerable<Guid> ids)
    {
        if (!ids.Any())
        {
            return Json(new { success = false, message = "No Tables Selected to Delete" });
        }

        Guid deletorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _tableAndSectionService.DeleteManyTableAsync(ids, deletorId);
        if (!response.Success)
        {
            return Json(new { success = false, message = response.Message });
        }

        return Json(new { success = true, message = response.Message });
    }
    #endregion Tables

    #region BindData

    public async Task<JsonResult> GetAllSections()
    {
        Response<IEnumerable<SectionVM>> response = await _tableAndSectionService.GetAllSectionsAsync();

        IEnumerable<SectionVM>? sectionlist = response.Data;
        SelectList sections = new(sectionlist, "Id", "Name");

        return Json(new { sections });
    }

    #endregion BindData

}
