using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ModifierGroupRepository : IModifierGroupRepository
{
    private readonly PizzashopContext _context;

    public ModifierGroupRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ModifierGroup>> GetAllAsync()
    {
        return await _context.ModifierGroups.Where(mg => !mg.IsDeleted).OrderBy(mg => mg.Preference).ThenBy(mg => mg.Name).ToListAsync();
    }

    public async Task<ModifierGroup?> GetByIdAsync(Guid id)
    {
        return await _context.ModifierGroups.SingleOrDefaultAsync(mg => mg.Id == id && !mg.IsDeleted);
    }

    public async Task<bool> IsExistsAsync(Expression<Func<ModifierGroup, bool>> predicate)
    {
        return await _context.ModifierGroups.AnyAsync(predicate);
    }

    public async Task AddAsync(ModifierGroup modifierGroup)
    {
        _context.ModifierGroups.Add(modifierGroup);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ModifierGroup modifierGroup)
    {
        _context.ModifierGroups.Update(modifierGroup);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<ModifierGroup> modifierGroups)
    {
        _context.ModifierGroups.UpdateRange(modifierGroups);
        await _context.SaveChangesAsync();
    }
}
