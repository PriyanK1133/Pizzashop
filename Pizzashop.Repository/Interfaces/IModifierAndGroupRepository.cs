using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IModifierAndGroupRepository
{
    Task<IEnumerable<Modifier>> GetModifiersByGroupIdAsync(Guid modifierGroupId);
    Task<IEnumerable<ModifierGroup>> GetModifierGroupsByModifierIdAsync(Guid modifierId);
    Task<IEnumerable<ModifiersAndGroup>> GetModifiersAndGroupByGroupIdAsync(Guid modifierGroupId);
    Task<IEnumerable<ModifiersAndGroup>> GetModifiersAndGroupByModifierIdAsync(Guid modifierId);
    Task<(IEnumerable<Modifier> list, int totalRecords)> GetPagedModifiersAsync(Guid modifierGroupId, string searchString, int page, int pagesize, bool isASC);
    Task AddRangeAsync(IEnumerable<ModifiersAndGroup> modifiersAndGroups);
    Task UpdateRangeAsync(IEnumerable<ModifiersAndGroup> modifiersAndGroups);
    Task DeleteRangeAsync(IEnumerable<ModifiersAndGroup> modifiersAndGroups);
}