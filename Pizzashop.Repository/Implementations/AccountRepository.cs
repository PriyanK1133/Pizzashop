using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class AccountRepository : IAccountRepository
{
    private readonly PizzashopContext _context;

    public AccountRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<bool> IsExistByIdAsync(Guid id){
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id) != null;
    }
    public async Task<bool> IsExistByEmailAsync(string email){
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email) != null;
    }

    public async Task<Account?> GetByIdAsync(Guid id){
        return await _context.Accounts.Include(a => a.Role).Include(a => a.User).SingleOrDefaultAsync(a => a.Id == id && !a.User.IsDeleted);
    }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        return await _context.Accounts.Include(a => a.Role).Include(a => a.User).SingleOrDefaultAsync(a => a.Email == email && !a.User.IsDeleted);
    }

    public async Task AddAsync(Account account){
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account){
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id){
        var account = await _context.Accounts.SingleOrDefaultAsync(u => u.Id == id);
        _context.Accounts.Remove(account!);
        await _context.SaveChangesAsync();
    }
}
