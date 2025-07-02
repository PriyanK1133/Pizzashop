using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IModifierGroupRepository
{
    Task<ModifierGroup?> GetByIdAsync(Guid id);
    Task<IEnumerable<ModifierGroup>> GetAllAsync();
    Task<bool> IsExistsAsync(Expression<Func<ModifierGroup, bool>> predicate);
    Task AddAsync(ModifierGroup modifierGroup);
    Task UpdateAsync(ModifierGroup modifierGroup);
    Task UpdateRangeAsync(List<ModifierGroup> modifierGroups);
}
