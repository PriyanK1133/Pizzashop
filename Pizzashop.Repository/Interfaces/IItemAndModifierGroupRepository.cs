using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IItemAndModifierGroupRepository
{

    Task<IEnumerable<ItemsAndModifierGroup>> GetByItemId(Guid itemId);
    Task<IEnumerable<ItemsAndModifierGroup>> GetApplicableModifierByItemId(Guid itemId);
    Task AddRangeAsync(List<ItemsAndModifierGroup> itemsAndModifierGroups);
    Task UpdateRangeAsync(List<ItemsAndModifierGroup> itemsAndModifierGroups);
    Task RemoveRangeAsync(List<ItemsAndModifierGroup> itemsAndModifierGroups);
}
