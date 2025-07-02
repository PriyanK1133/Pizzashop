using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class PaymentRepository : IPaymentRepository
{
    private readonly PizzashopContext _context;

    public PaymentRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Payments.SingleOrDefaultAsync(p => p.Id == orderId);
    }

    public async Task AddAsync(Payment payment){
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

}
