using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface ICustomersService
{
    Task<Response<CustomerHistoryVM?>> GetByIdAsync(Guid id);
    Task<Response<PagedResult<CustomerVM>>> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, DateOnly? fromDate, DateOnly? toDate);
    Task<Response<MemoryStream>> ExportCustomersAsync(string searchString, DateOnly? fromDate, DateOnly? toDate, string account);

}
