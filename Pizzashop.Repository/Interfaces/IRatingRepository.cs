using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IRatingRepository
{
    Task<Rating?> GetByOrderIdAsync(Guid orderId);
    Task AddAsync(Rating rating);
}
