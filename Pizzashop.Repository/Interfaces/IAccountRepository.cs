using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IAccountRepository
{
    Task<bool> IsExistByIdAsync(Guid id);
    Task<bool> IsExistByEmailAsync(string email);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetByEmailAsync(string email);
    Task UpdateAsync(Account account);
    Task AddAsync(Account account);
    Task DeleteAsync(Guid id);
    
}
