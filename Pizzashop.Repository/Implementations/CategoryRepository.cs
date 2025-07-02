using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly PizzashopContext _context;

    public CategoryRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Preference).ThenBy(c => c.Name).ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories.SingleOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }
    
    public async Task<bool> IsExistsAsync(Expression<Func<Category, bool>> predicate)
    {
        return await _context.Categories.AnyAsync(predicate);
    }

    public async Task AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<Category> categories){
        _context.Categories.UpdateRange(categories);
        await _context.SaveChangesAsync();
    }

}
