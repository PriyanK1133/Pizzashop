using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderTableRepository : IOrderTableRepository
{
    private readonly PizzashopContext _context;

    public OrderTableRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<IEnumerable<TableOrderMapping>> GetByOrderIdAsync(Guid orderId){
        return await _context.TableOrderMappings.Where(ot => ot.OrderId == orderId).Include(ot => ot.Table).ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<TableOrderMapping> orderTables)
    {
        await _context.TableOrderMappings.AddRangeAsync(orderTables);
        await _context.SaveChangesAsync();
    }

}
