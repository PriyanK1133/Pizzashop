using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ModifierRepository : IModifierRepository
{
    private readonly PizzashopContext _context;

    public ModifierRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Modifier?> GetByIdAsync(Guid id)
    {
        return await _context.Modifiers.SingleOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
    }

    public async Task<IEnumerable<Modifier>> GetAllAsync()
    {
        return await _context.Modifiers.Where(m => !m.IsDeleted).Include(m => m.Unit).ToListAsync();
    }

    public async Task<(IEnumerable<Modifier> list, int totalRecords)> GetAllPagedModifiersAsync(string searchString, int page, int pagesize, bool isASC)
    {
        IQueryable<Modifier> query = _context.Modifiers.Where(i => !i.IsDeleted).Include(i => i.Unit);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (isASC)
        {
            query.OrderBy(i => i.Name);
        }
        else
        {
            query.OrderByDescending(i => i.Name);
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());
    }

    public async Task AddAsync(Modifier modifier)
    {
        await _context.Modifiers.AddAsync(modifier);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Modifier modifier)
    {
        _context.Modifiers.Update(modifier);
        await _context.SaveChangesAsync();
    }

}
