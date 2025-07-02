using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class ItemAndModifierGroupRepository : IItemAndModifierGroupRepository
{
    private readonly PizzashopContext _context;
    
    public ItemAndModifierGroupRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<IEnumerable<ItemsAndModifierGroup>> GetByItemId(Guid itemId)
    {
        return await _context.ItemsAndModifierGroups.Where(i => i.ItemId == itemId).OrderBy(i => i.ModifierGroupId).ToListAsync();
    }

    public async Task<IEnumerable<ItemsAndModifierGroup>> GetApplicableModifierByItemId(Guid itemId){
        return await _context.ItemsAndModifierGroups.Where(im => im.ItemId == itemId).Include(im => im.ModifierGroup.ModifiersAndGroups.Where(mg => !mg.IsDeleted && !mg.Modifier.IsDeleted)).ThenInclude(mg => mg.Modifier).ToListAsync();
    }

    public async Task AddRangeAsync(List<ItemsAndModifierGroup> itemsAndModifierGroups)
    {
        await _context.ItemsAndModifierGroups.AddRangeAsync(itemsAndModifierGroups);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<ItemsAndModifierGroup> itemsAndModifierGroups)
    {
        _context.ItemsAndModifierGroups.RemoveRange(itemsAndModifierGroups);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<ItemsAndModifierGroup> itemsAndModifierGroups)
    {
        _context.ItemsAndModifierGroups.UpdateRange(itemsAndModifierGroups);
        await _context.SaveChangesAsync();
    }
}
