using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class SectionRepository : ISectionRepository
{
    private readonly PizzashopContext _context;

    public SectionRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<Section?> GetByIdAsync(Guid id)
    {
        return await _context.Sections.SingleOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<IEnumerable<Section>> GetAllAsync()
    {
        return await _context.Sections.Where(s => !s.IsDeleted).OrderBy(s => s.Preference).ToListAsync();
    }

    public async Task<IEnumerable<SectionVM>> GetAllSectionsWithStatusWiseTableCountAsync()
    {
        IQueryable<Section> query = _context.Sections.Where(s => !s.IsDeleted).OrderBy(s => s.Preference);

        IEnumerable<SectionVM> sections = await query.Select(s => new SectionVM()
        {
            Id = s.Id,
            Name = s.Name,
            AssignedTables = s.Tables.Count(t => !t.IsDeleted && t.CurrentOrder != null && t.CurrentOrder.Status == Constants.OrderPending),
            OccupiedTables = s.Tables.Count(t => !t.IsDeleted && t.CurrentOrder != null && (t.CurrentOrder.Status == Constants.OrderInProgress || t.CurrentOrder.Status == Constants.OrderServed)),
            AvailableTables = s.Tables.Count(t => !t.IsDeleted && !t.IsOccupied),
            WaitingCustomers = s.CustomerVisitDetails.Count(cv => cv.IsWaiting),
        }).ToListAsync();

        return sections;
    }

    public async Task AddAsync(Section section)
    {
        await _context.Sections.AddAsync(section);
        await _context.SaveChangesAsync();

    }

    public async Task UpdateAsync(Section section)
    {
        _context.Sections.Update(section);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<Section, bool>> predicate)
    {
        return await _context.Sections.AnyAsync(predicate);
    }

    public async Task UpdateRangeAsync(List<Section> sections)
    {
        _context.Sections.UpdateRange(sections);
        await _context.SaveChangesAsync();
    }

}
