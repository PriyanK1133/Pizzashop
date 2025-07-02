using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class TableRepository : ITableRepository
{
    private readonly PizzashopContext _context;

    public TableRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Table table)
    {
        await _context.Tables.AddAsync(table);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Table>> GetAllBySectionIdAsync(Guid sectionId)
    {
        return await _context.Tables.Where(t => t.SectionId == sectionId && !t.IsDeleted).ToListAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<Table, bool>> predicate)
    {
        return await _context.Tables.AnyAsync(predicate);
    }

    public async Task<(IEnumerable<Table> list, int totalRecords)> GetPagedTablesAsync(Guid sectionId, string searchString, int page, int pagesize, bool isASC)
    {
        IQueryable<Table> query = _context.Tables.Where(i => i.SectionId == sectionId && !i.IsDeleted);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (isASC)
        {
            query = query.OrderBy(i => i.Name);
        }
        else
        {
            query = query.OrderByDescending(i => i.Name);
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }

    public async Task<Table?> GetByIdAsync(Guid id)
    {
        return await _context.Tables.Include(t => t.Section).SingleOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task UpdateAsync(Table table)
    {
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<Table> tables)
    {
        _context.Tables.UpdateRange(tables);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Table>> GetTablesWithOrderDetailsBySectionIdAsync(Guid sectionId)
    {
        return await _context.Tables.Include(t => t.CurrentOrder).Where(t => !t.IsDeleted && t.SectionId == sectionId).ToListAsync();
    }

    public async Task<IEnumerable<Table>> GetAvailableTablesBySectionIdAsync(Guid sectionId){
        return await _context.Tables.Where(t => !t.IsDeleted && t.SectionId == sectionId && !t.IsOccupied).ToListAsync();
    }

}
