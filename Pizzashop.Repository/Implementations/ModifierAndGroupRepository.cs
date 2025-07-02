using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ModifierAndGroupRepository : IModifierAndGroupRepository
{
    private readonly PizzashopContext _context;

    public ModifierAndGroupRepository(PizzashopContext context)
    {
        _context = context;
    }

    #region  Modifiers

    public async Task<IEnumerable<Modifier>> GetModifiersByGroupIdAsync(Guid modifierGroupId)
    {
        return await _context.ModifiersAndGroups.Where(mag => mag.ModifierGroupId == modifierGroupId).Include(mag => mag.Modifier).ThenInclude(m => m.Unit).Select(mag => mag.Modifier).ToListAsync();
    }

    public async Task<(IEnumerable<Modifier> list, int totalRecords)> GetPagedModifiersAsync(Guid modifierGroupId, string searchString, int page, int pagesize, bool isASC)
    {
        IQueryable<Modifier> query;
        if (!modifierGroupId.Equals(Guid.Empty))
        {
            query = _context.ModifiersAndGroups.Where(m => m.ModifierGroupId == modifierGroupId && !m.IsDeleted).Include(m => m.Modifier).ThenInclude(m => m.Unit).Select(m => m.Modifier);
        }
        else
        {
            query = _context.ModifiersAndGroups.Where(m => !m.IsDeleted).Include(m => m.Modifier).ThenInclude(m => m.Unit).Select(m => m.Modifier);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(searchString));
        }

        if (isASC)
        {
            query.OrderBy(m => m.Name);
        }
        else
        {
            query.OrderByDescending(m => m.Name);
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());

    }

    #endregion

    #region  ModifiersAndGroups

    public async Task<IEnumerable<ModifierGroup>> GetModifierGroupsByModifierIdAsync(Guid modifierId)
    {
        return await _context.ModifiersAndGroups.Where(mag => mag.ModifierId == modifierId).Include(mag => mag.ModifierGroup).Select(mag => mag.ModifierGroup).ToListAsync();
    }

    public async Task<IEnumerable<ModifiersAndGroup>> GetModifiersAndGroupByGroupIdAsync(Guid modifierGroupId)
    {
        return await _context.ModifiersAndGroups.Where(mag => mag.ModifierGroupId == modifierGroupId).ToListAsync();
    }

    public async Task<IEnumerable<ModifiersAndGroup>> GetModifiersAndGroupByModifierIdAsync(Guid modifierId)
    {
        return await _context.ModifiersAndGroups.Where(mag => mag.ModifierId == modifierId).ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<ModifiersAndGroup> modifiersAndGroups)
    {
        await _context.ModifiersAndGroups.AddRangeAsync(modifiersAndGroups);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<ModifiersAndGroup> modifiersAndGroups)
    {
        _context.ModifiersAndGroups.UpdateRange(modifiersAndGroups);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<ModifiersAndGroup> modifiersAndGroups)
    {
        _context.ModifiersAndGroups.RemoveRange(modifiersAndGroups);
        await _context.SaveChangesAsync();
    }

    #endregion
}
