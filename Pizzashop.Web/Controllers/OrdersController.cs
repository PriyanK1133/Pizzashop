using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Attributes;
using Rotativa.AspNetCore;

namespace Pizzashop.Web.Controllers;

[PermissionAuthorize(Constants.CanView)]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetPagedOrders(DateOnly? fromDate, DateOnly? toDate, string searchString = "", int page = 1, int pagesize = 5, string sortColumn = "", bool isASC = true, string status = "")
    {
        Response<PagedResult<OrderListVM>> response = await _orderService.GetPagedListAsync(searchString, page, pagesize, sortColumn, isASC, status, fromDate, toDate);
        if (!response.Success)
        {
            return PartialView("_Orders", new PagedResult<OrderListVM>());
        }

        return PartialView("_Orders", response.Data!);
    }

    public async Task<IActionResult> ExportOrders(string pastDays = "month", string status = "", string searchString = "")
    {
        DateOnly? fromDate = null, toDate = null;
        DateTime date = DateTime.Today;
        if (pastDays.Equals("month"))
        {
            toDate = DateOnly.FromDateTime(date);
            fromDate = new(date.Year, date.Month, 1);
        }
        else if (!string.IsNullOrEmpty(pastDays) && !pastDays.Equals("all"))
        {
            toDate = DateOnly.FromDateTime(date);

            int past = int.Parse(pastDays) * -1;
            fromDate = DateOnly.FromDateTime(date.AddDays(past));
        }

        Response<MemoryStream> response = await _orderService.ExportOrdersAsync(searchString, status, fromDate, toDate);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        MemoryStream fileStream = response.Data!;
        return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task<IActionResult> OrderDetails(Guid id)
    {
        Response<OrderDetailsVM?> response = await _orderService.GetByIdAsync(id);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        return View(response.Data!);
    }

    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        Response<OrderDetailsVM?> response = await _orderService.GetByIdAsync(id);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }
        return new ViewAsPdf("OrderPdf", response.Data)
        {
            FileName = "Invoice.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageMargins = new Rotativa.AspNetCore.Options.Margins(0, 0, 0, 0),
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
        };

    }

}