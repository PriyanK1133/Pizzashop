using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Web.Controllers;

[Authorize(Roles = Constants.AccountManager + "," + Constants.Admin + "," + Constants.SuperAdmin)]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetDashboardData(DateOnly? fromDate, DateOnly? toDate)
    {
        Response<DashboardVM?> response = await _dashboardService.GetDashboardDataAsync(fromDate, toDate);

        if (!response.Success)
        {
            return PartialView("_Dashboard", new DashboardVM());
        }

        return PartialView("_Dashboard", response.Data);
    }

    public async Task<JsonResult> GetGraphData(DateOnly? fromDate, DateOnly? toDate)
    {
        Response<GraphDataVM?> response = await _dashboardService.GetGraphDataAsync(fromDate, toDate);
        return Json(response);
    }
}
