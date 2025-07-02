using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IDashboardService
{
    Task<Response<DashboardVM?>> GetDashboardDataAsync(DateOnly? fromDate, DateOnly? toDate);
    Task<Response<GraphDataVM?>> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate);
}
