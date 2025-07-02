using System.Linq.Expressions;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.DTO;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<(IEnumerable<GraphDataDTO> revenueData, IEnumerable<GraphDataDTO> customerGrowthData)> GetGraphDataAsync(DateOnly? fromDate, DateOnly? toDate);
    Task<OrderSummaryDTO> GetOrderSummaryAsync(DateOnly? fromDate, DateOnly? toDate);
    Task<(IEnumerable<Order> orders, int totalRecords)> GetOrdersAsync(string searchString, string status, DateOnly? fromDate, DateOnly? toDate);
    Task<IEnumerable<Order>> GetKOTOrdersAsync(Guid categoryId);
    Task<(IEnumerable<Order> list, int totalRecords)> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, string status, DateOnly? fromDate, DateOnly? toDate);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task<bool> IsExistsAsync(Expression<Func<Order, bool>> predicate);
    Task UpdateKOTItemsAsync(Guid orderId, List<string> orderItemIds, string status, List<int> quantities);
}
