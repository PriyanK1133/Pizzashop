using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IOrderService
{
    Task<Response<OrderDetailsVM?>> GetByIdAsync(Guid id);
    Task<Response<PagedResult<OrderListVM>>> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, string status, DateOnly? fromDate, DateOnly? toDate);
    Task<Response<MemoryStream>> ExportOrdersAsync(string searchString, string status, DateOnly? fromDate, DateOnly? toDate);
}
