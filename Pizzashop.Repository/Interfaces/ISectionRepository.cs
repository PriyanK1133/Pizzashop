using System.Linq.Expressions;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;

namespace Pizzashop.Repository.Interfaces;

public interface ISectionRepository
{
    Task<IEnumerable<Section>> GetAllAsync();
    Task<Section?> GetByIdAsync(Guid id);
    Task<bool> IsExistsAsync(Expression<Func<Section, bool>> predicate);
    Task AddAsync(Section section);
    Task UpdateAsync(Section section);
    Task UpdateRangeAsync(List<Section> sections);
    Task<IEnumerable<SectionVM>> GetAllSectionsWithStatusWiseTableCountAsync();
}
