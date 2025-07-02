using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ITaxAndFeeRepository
{
    Task<TaxesAndFee?> GetByIdAsync(Guid id);
    Task<IEnumerable<TaxesAndFee>> GetAllAsync();
    Task<IEnumerable<TaxesAndFee>> GetEnabledAsync();
    Task<(IEnumerable<TaxesAndFee> list, int totalRecords)> GetPagedListAsync(string searchString, int page, int pagesize, bool isASC);
    Task AddAsync(TaxesAndFee taxesAndFee);
    Task UpdateAsync(TaxesAndFee taxesAndFee);
}
