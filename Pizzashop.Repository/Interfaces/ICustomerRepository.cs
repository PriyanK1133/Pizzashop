using System.Linq.Expressions;
using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<bool> IsExistsAsync(Expression<Func<Customer, bool>> predicate);

    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<(IEnumerable<Customer> list, int totalRecords)> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, DateOnly? fromDate, DateOnly? toDate);
    Task<IEnumerable<Customer>> GetCustomersAsync(string searchString, DateOnly? fromDate, DateOnly? toDate);
    Task<int> GetNewCustomerCountAsync(DateOnly? fromDate, DateOnly? toDate);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task RemoveAsync(Customer customer);
}
