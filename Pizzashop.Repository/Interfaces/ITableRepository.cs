using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ITableRepository
{
    Task<Table?> GetByIdAsync(Guid id);
    Task<IEnumerable<Table>> GetAllBySectionIdAsync(Guid sectionId);
    Task<bool> IsExistsAsync(Expression<Func<Table, bool>> predicate);
    Task<(IEnumerable<Table> list, int totalRecords)> GetPagedTablesAsync(Guid sectionId, string searchString, int page, int pagesize, bool isASC);
    Task AddAsync(Table table);
    Task UpdateAsync(Table table);
    Task UpdateRangeAsync(IEnumerable<Table> tables);
    Task<IEnumerable<Table>> GetTablesWithOrderDetailsBySectionIdAsync(Guid sectionId);
    Task<IEnumerable<Table>> GetAvailableTablesBySectionIdAsync(Guid sectionId);
}
