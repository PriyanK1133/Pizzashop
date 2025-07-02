using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface ITaxAndFeeService
{

    Task<Response<TaxAndFeeVM?>> GetById(Guid id);
    Task<Response<IEnumerable<TaxAndFeeVM>>> GetAllAsync();
    Task<Response<PagedResult<TaxAndFeeVM>>> GetPagedListAsync(string searchString, int page, int pagesize, bool isASC);
    Task<Response<TaxAndFeeVM?>> AddAsync(TaxAndFeeVM model);
    Task<Response<TaxAndFeeVM?>> EditAsync(TaxAndFeeVM model);
    Task<Response<bool>> DeleteAsync(Guid id, Guid deltorId);
}
