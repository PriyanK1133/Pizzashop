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
public class RoleAndPermissionController : Controller
{
    private readonly IRoleAndPermissionService _roleAndPermissionService;

    public RoleAndPermissionController(IRoleAndPermissionService roleAndPermissionService)
    {
        _roleAndPermissionService = roleAndPermissionService;
    }

    public async Task<IActionResult> Index()
    {
        Response<IEnumerable<RoleVM>> roleResponse = await _roleAndPermissionService.GetAllRolesAsync();
        return View(roleResponse.Data);
    }

    public async Task<IActionResult> Permissions(Guid id)
    {
        Response<RoleAndPermissionVM> response = await _roleAndPermissionService.GetPermissionsForRoleAsync(id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        Response<IEnumerable<RoleVM>> roles = await _roleAndPermissionService.GetAllRolesAsync();
        ViewBag.Roles = new SelectList(roles.Data, "Id", "Name");

        return View(response.Data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> Permissions(RoleAndPermissionVM model)
    {
        Guid updatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<RoleAndPermissionVM> response = await _roleAndPermissionService.UpdateAsync(model, updatorId);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return View(model);
        }
        TempData["success"] = response.Message;
        return RedirectToAction("Index");
    }
}
