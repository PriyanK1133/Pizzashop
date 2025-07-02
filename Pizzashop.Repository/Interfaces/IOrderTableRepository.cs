using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IOrderTableRepository
{
    Task<IEnumerable<TableOrderMapping>> GetByOrderIdAsync(Guid orderId);
    Task AddRangeAsync(IEnumerable<TableOrderMapping> orderTables);
}
