using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IModifierRepository
{
    Task<Modifier?> GetByIdAsync(Guid id);
    Task<IEnumerable<Modifier>> GetAllAsync();
    Task<(IEnumerable<Modifier> list, int totalRecords)> GetAllPagedModifiersAsync(string searchString, int page, int pagesize, bool isASC);
    Task AddAsync(Modifier modifier);
    Task UpdateAsync(Modifier modifier);
}
