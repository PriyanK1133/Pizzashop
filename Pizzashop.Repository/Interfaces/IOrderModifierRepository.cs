using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderModifierRepository
{
    Task<OrderModifier?> GetByIdAsync(Guid id);
    Task<IEnumerable<OrderModifier>> GetAllByOrderItemIdAsync(Guid orderItemId);
    Task AddAsync(OrderModifier orderModifier);
    Task AddRangeAsync(List<OrderModifier> orderModifiers);
    Task UpdateAsync(OrderModifier orderModifier);
    Task UpdateRangeAsync(List<OrderModifier> orderModifiers);
    Task RemoveAsync(OrderModifier orderModifier);
    Task RemoveRangeAsync(List<OrderModifier> orderModifiers);
}
