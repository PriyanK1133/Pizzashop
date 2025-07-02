using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderTaxRepository : IOrderTaxRepository
{
    private readonly PizzashopContext _context;

    public OrderTaxRepository(PizzashopContext context){
        _context = context;
    }

    public async Task AddRangeAsync(List<OrderTaxis> orderTaxes)
    {
        await _context.OrderTaxes.AddRangeAsync(orderTaxes);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderTaxis>> GetAllByOrderId(Guid orderId)
    {
        return await _context.OrderTaxes.Where(ot => ot.OrderId == orderId).ToListAsync();
    }

    public async Task<OrderTaxis?> GetById(Guid id)
    {
        return await _context.OrderTaxes.SingleOrDefaultAsync(ot => ot.Id == id);
    }

    public async Task UpdateRangeAsync(List<OrderTaxis> orderTaxes)
    {
        _context.OrderTaxes.UpdateRange(orderTaxes);
        await _context.SaveChangesAsync();
    }

}
