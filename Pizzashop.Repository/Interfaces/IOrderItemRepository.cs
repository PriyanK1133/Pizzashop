using Pizzashop.Entity.Data;
using Pizzashop.Entity.DTO;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderItemRepository
{
    Task<IEnumerable<OrderItem>> GetAllByOrderIdAsync(Guid orderId);
    Task<int> GetReadyQuantityAsync(Guid id);
    Task<OrderItem?> GetByIdAsync(Guid id);
    Task<string?> GetSpecialInstructionByIdAsync(Guid id);
    Task<(IEnumerable<ItemSummaryDTO> mostSellingItems, IEnumerable<ItemSummaryDTO> leastSellingItems)> GetItemsSummary(DateOnly? fromDate, DateOnly? toDate);
    Task AddAsync(OrderItem orderItem);
    Task AddRangeAsync(List<OrderItem> orderItems);
    Task UpdateAsync(OrderItem orderItem);
    Task UpdateRangeAsync(List<OrderItem> orderItems);
    Task RemoveAsync(OrderItem orderItem);
    Task RemoveRangeAsync(List<OrderItem> orderItems);
}
