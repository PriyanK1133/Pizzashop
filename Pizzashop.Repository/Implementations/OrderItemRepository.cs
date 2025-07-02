using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.DTO;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly PizzashopContext _context;

    public OrderItemRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<OrderItem?> GetByIdAsync(Guid id){
        return await _context.OrderItems.SingleOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<IEnumerable<OrderItem>> GetAllByOrderIdAsync(Guid orderId){
        return await _context.OrderItems.Where(i =>i.OrderId == orderId).OrderBy(i => i.CreatedAt).ToListAsync();
    }

    public async Task<string?> GetSpecialInstructionByIdAsync(Guid id){
        return (await _context.OrderItems.SingleOrDefaultAsync(i => i.Id == id))?.SpecialInstruction;
    }

    public async Task<int> GetReadyQuantityAsync(Guid id){
        return await _context.OrderItems.Where(i => i.Id == id).Select(i => i.ReadyQuantity.GetValueOrDefault()).SingleOrDefaultAsync();
    }
    
    public async Task<(IEnumerable<ItemSummaryDTO> mostSellingItems, IEnumerable<ItemSummaryDTO> leastSellingItems)> GetItemsSummary(DateOnly? fromDate, DateOnly? toDate){
        IQueryable<OrderItem> query = _context.OrderItems;

        if(fromDate != null){
            query = query.Where(oi => DateOnly.FromDateTime(oi.CreatedAt) >= fromDate);
        }

        if(toDate != null){
            query = query.Where(oi => DateOnly.FromDateTime(oi.CreatedAt) <= toDate);
        }

        IQueryable<ItemSummaryDTO>  queryItems = query.GroupBy(oi => oi.Item).OrderByDescending(oi => oi.Count()).Select(i => new ItemSummaryDTO(){
            Id = i.Key.Id,
            Name = i.Key.Name,
            Image = i.Key.Image,
            OrderCount = i.Count()
        });

        List<ItemSummaryDTO> mostSellingItems = await queryItems.Take(5).ToListAsync();
        List<ItemSummaryDTO> leastSellingItems = await queryItems.Reverse().Take(5).ToListAsync();

        return (mostSellingItems, leastSellingItems);
    }

    public async Task AddAsync(OrderItem orderItem){
        await _context.OrderItems.AddAsync(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<OrderItem> orderItems){
        await _context.OrderItems.AddRangeAsync(orderItems);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(OrderItem orderItem)
    {
        _context.OrderItems.Update(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<OrderItem> orderItems){
        _context.OrderItems.UpdateRange(orderItems);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(OrderItem orderItem){
        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<OrderItem> orderItems){
        _context.OrderItems.RemoveRange(orderItems);
        await _context.SaveChangesAsync();
    }

}
