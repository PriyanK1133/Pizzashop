using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderModifierRepository : IOrderModifierRepository
{
    private readonly PizzashopContext _context;

    public OrderModifierRepository(PizzashopContext context){
        _context = context;
    }

    public async Task AddAsync(OrderModifier orderModifier)
    {
        await _context.OrderModifiers.AddAsync(orderModifier);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<OrderModifier> orderModifiers)
    {
        await _context.OrderModifiers.AddRangeAsync(orderModifiers);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderModifier>> GetAllByOrderItemIdAsync(Guid orderItemId)
    {
        return await _context.OrderModifiers.Where(om => om.OrderItemId == orderItemId).ToListAsync();
    }

    public async Task<OrderModifier?> GetByIdAsync(Guid id)
    {
        return await _context.OrderModifiers.SingleOrDefaultAsync(om => om.Id == id);
    }

    public async Task UpdateAsync(OrderModifier orderModifier)
    {
        _context.OrderModifiers.Update(orderModifier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<OrderModifier> orderModifiers)
    {
        _context.OrderModifiers.UpdateRange(orderModifiers);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(OrderModifier orderModifier){
        _context.OrderModifiers.Remove(orderModifier);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<OrderModifier> orderModifiers){
        _context.OrderModifiers.RemoveRange(orderModifiers);
        await _context.SaveChangesAsync();
    }
}
