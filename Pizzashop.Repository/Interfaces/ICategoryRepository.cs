using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<bool> IsExistsAsync(Expression<Func<Category, bool>> predicate);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task UpdateRangeAsync(List<Category> categories);
}
