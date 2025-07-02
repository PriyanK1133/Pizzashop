using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class RatingRepository : IRatingRepository
{
    private readonly PizzashopContext _context;

    public RatingRepository(PizzashopContext context) {
        _context = context;
    }

    public async Task<Rating?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Ratings.SingleOrDefaultAsync(r => r.OrderId == orderId);
    }

    public async Task AddAsync(Rating rating){
        await _context.Ratings.AddAsync(rating);
        await _context.SaveChangesAsync();
    }

}
