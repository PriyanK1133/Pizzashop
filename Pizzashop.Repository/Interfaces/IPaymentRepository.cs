using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
    Task AddAsync(Payment payment);
}
