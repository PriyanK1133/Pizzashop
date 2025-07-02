using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Utils;
using Pizzashop.Web.Attributes;

namespace Pizzashop.Web.Controllers;

[PermissionAuthorize(Constants.CanView)]
public class CustomersController : Controller
{
    private readonly ICustomersService _customersService;

    public CustomersController(ICustomersService customersService)
    {
        _customersService = customersService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetPagedCustomers(DateOnly? fromDate, DateOnly? toDate, string searchString = "", int page = 1, int pagesize = 5, string sortColumn = "", bool isASC = true)
    {
        Response<PagedResult<CustomerVM>> response = await _customersService.GetPagedListAsync(searchString, page, pagesize, sortColumn, isASC, fromDate, toDate);
        if (!response.Success)
        {
            return PartialView("_Customers", new PagedResult<CustomerVM>());
        }

        return PartialView("_customers", response.Data!);
    }

    public async Task<IActionResult> ExportCustomers(DateOnly? fromDate, DateOnly? toDate, string searchString = "")
    {

        UserVM user = SessionUtils.GetUser(HttpContext)!;
        string account = user.Role!;
        
        Response<MemoryStream> response = await _customersService.ExportCustomersAsync(searchString, fromDate, toDate, account);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        MemoryStream fileStream = response.Data!;
        return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task<IActionResult> GetById(Guid id)
    {
        Response<CustomerHistoryVM?> response = await _customersService.GetByIdAsync(id);

        if(!response.Success){
            return Json(new {success = false, message = response.Message});
        }

        return PartialView("_CustomerHistory", response.Data!);
    }

}
