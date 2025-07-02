using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateAsync(User user);
    Task<bool> IsExistsAsync(Expression<Func<User, bool>> predicate);
    Task AddAsync(User user);
    Task<(IEnumerable<User> list, int totalRecords)> GetPagedUsersAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC);
}
