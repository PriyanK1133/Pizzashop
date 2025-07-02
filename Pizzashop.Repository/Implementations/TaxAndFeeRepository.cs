using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class TaxAndFeeRepository : ITaxAndFeeRepository
{
    private readonly PizzashopContext _context;
    public TaxAndFeeRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<TaxesAndFee?> GetByIdAsync(Guid id)
    {
        return await _context.TaxesAndFees.SingleOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
    }

    public async Task<IEnumerable<TaxesAndFee>> GetAllAsync()
    {
        return await _context.TaxesAndFees.Where(t => !t.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<TaxesAndFee>> GetEnabledAsync(){
        return await _context.TaxesAndFees.Where(t => (t.IsEnabled ?? false) && !t.IsDeleted).ToListAsync();
    }

    public async Task<(IEnumerable<TaxesAndFee> list, int totalRecords)> GetPagedListAsync(string searchString, int page, int pagesize, bool isASC)
    {
        IQueryable<TaxesAndFee> query = _context.TaxesAndFees.Where(i => !i.IsDeleted);
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

    public async Task AddAsync(TaxesAndFee taxesAndFee)
    {
        await _context.TaxesAndFees.AddAsync(taxesAndFee);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TaxesAndFee taxesAndFee)
    {
        _context.TaxesAndFees.Update(taxesAndFee);
        await _context.SaveChangesAsync();
    }

}
