using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderTaxRepository
{
    Task<IEnumerable<OrderTaxis>> GetAllByOrderId(Guid orderId);
    Task<OrderTaxis?> GetById(Guid id);
    Task AddRangeAsync(List<OrderTaxis> orderTaxes);
    Task UpdateRangeAsync(List<OrderTaxis> orderTaxes);
}
