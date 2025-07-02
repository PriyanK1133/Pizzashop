using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICustomerVisitDetailRepository
{
    Task<bool> IsExistsAsync(Expression<Func<CustomerVisitDetail, bool>> predicate);
    Task<CustomerVisitDetail?> GetByIdAsync(Guid id);
    Task<CustomerVisitDetail?> GetWaitingCustomerByEmailAsync(string email);
    CustomerVisitDetail? GetWaitingCustomerByEmailSP(string email);
    Task<int> GetWaitingCustomerCountAsync();
    Task<IEnumerable<CustomerVisitDetail>> GetWaitingListAsync(Guid sectionId);
    IEnumerable<CustomerVisitDetail> GetWaitingListBySP(Guid sectionId);
    Task AddAsync(CustomerVisitDetail customerVisitDetail);
    Task UpdateAsync(CustomerVisitDetail customerVisitDetail);
    Task RemoveAsync(CustomerVisitDetail customerVisitDetail);
    Task<string> RemoveBySP(Guid id);
}
