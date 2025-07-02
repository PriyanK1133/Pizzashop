using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly PizzashopContext _context;

    public CustomerRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<bool> IsExistsAsync(Expression<Func<Customer, bool>> predicate)
    {
        return await _context.Customers.AnyAsync(predicate);
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
                            .Include(c => c.CustomerVisitDetails).ThenInclude(cv => cv.Orders).ThenInclude(o => o.OrderItems)
                            .Include(c => c.CustomerVisitDetails).ThenInclude(cv => cv.Orders).ThenInclude(o => o.Payments)
                            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByEmailAsync(string email){
        return await _context.Customers.SingleOrDefaultAsync(c => c.Email.ToLower().Trim() == email.ToLower().Trim());
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.Where(c => c.CustomerVisitDetails.Count(cv => !cv.IsWaiting) > 0).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync(string searchString, DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Customer> query = _context.Customers.Where(c => c.CustomerVisitDetails.Count(cv => !cv.IsWaiting) > 0).Include(i => i.CustomerVisitDetails).ThenInclude(c => c.Orders);

        if (fromDate != null)
        {
            query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) <= toDate);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        return await query.ToListAsync();
    }

    public async Task<int> GetNewCustomerCountAsync(DateOnly? fromDate, DateOnly? toDate){
        IQueryable<Customer> query = _context.Customers;

        if(fromDate != null){
            query = query.Where(c => DateOnly.FromDateTime(c.CreatedAt) >= fromDate);
        }

        if(toDate != null){
            query = query.Where(c => DateOnly.FromDateTime(c.CreatedAt) <= toDate);
        }

        return await query.CountAsync();
    }

    public async Task<(IEnumerable<Customer> list, int totalRecords)> GetPagedListAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC, DateOnly? fromDate, DateOnly? toDate)
    {
        IQueryable<Customer> query = _context.Customers.Where(c => c.CustomerVisitDetails.Count(cv => !cv.IsWaiting) > 0).Include(i => i.CustomerVisitDetails).ThenInclude(c => c.Orders);

        if (fromDate != null)
        {
            query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) >= fromDate);
        }

        if (toDate != null)
        {
            query = query.Where(i => DateOnly.FromDateTime(i.CreatedAt) <= toDate);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower().Trim();
            query = query.Where(i => i.Name.ToLower().Contains(searchString));
        }

        if (isASC)
        {
            query = sortColumn.ToLower() switch
            {
                "name" => query = query.OrderBy(i => i.Name),
                "totalorder" => query = query.OrderBy(i => i.CustomerVisitDetails.Sum(cv => cv.Orders.Count)),
                "date" => query = query.OrderBy(i => i.CreatedAt),
                _ => query = query.OrderBy(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5)),
            };
        }
        else
        {
            query = sortColumn.ToLower() switch
            {
                "name" => query = query.OrderByDescending(i => i.Name),
                "totalorder" => query = query.OrderByDescending(i => i.CustomerVisitDetails.Sum(cv => cv.Orders.Count)),
                "date" => query = query.OrderByDescending(i => i.CreatedAt),
                _ => query = query.OrderByDescending(i => i.Id.ToString().Substring(i.Id.ToString().Length - 5)),
            };

        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(Customer customer){
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

}
