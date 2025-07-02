using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class CustomerVisitDetailRepository : ICustomerVisitDetailRepository
{
    private readonly PizzashopContext _context;

    public CustomerVisitDetailRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<bool> IsExistsAsync(Expression<Func<CustomerVisitDetail, bool>> predicate)
    {
        return await _context.CustomerVisitDetails.AnyAsync(predicate);
    }

    public async Task<CustomerVisitDetail?> GetByIdAsync(Guid id)
    {
        return await _context.CustomerVisitDetails.Include(cv => cv.Customer).SingleOrDefaultAsync(cv => cv.Id == id && !cv.IsDeleted);
    }

    public async Task<int> GetWaitingCustomerCountAsync()
    {
        return await _context.CustomerVisitDetails.Where(cv => cv.IsWaiting).CountAsync();
    }

    public async Task<CustomerVisitDetail?> GetWaitingCustomerByEmailAsync(string email)
    {
        return await _context.CustomerVisitDetails.Include(cv => cv.Customer).SingleOrDefaultAsync(cv => cv.Customer.Email.ToLower() == email.ToLower().Trim() && cv.IsWaiting && !cv.IsDeleted);
    }

    public CustomerVisitDetail? GetWaitingCustomerByEmailSP(string email)
    {
        string? result = _context.Database.SqlQueryRaw<string>($"select get_waiting_customer_by_email('{email}')").AsEnumerable().FirstOrDefault();

        if (string.IsNullOrEmpty(result))
        {
            return null;
        }

        CustomerVisitDetail? customerVisitDetail = JsonSerializer.Deserialize<CustomerVisitDetail>(result);

        return customerVisitDetail;
    }

    public async Task<IEnumerable<CustomerVisitDetail>> GetWaitingListAsync(Guid sectionId)
    {
        IQueryable<CustomerVisitDetail> query = _context.CustomerVisitDetails.Include(cv => cv.Customer).Where(cv => cv.IsWaiting && !cv.IsDeleted).OrderByDescending(cv => cv.UpdatedAt);

        if (sectionId != Guid.Empty)
        {
            query = query.Where(cv => cv.SectionId == sectionId);
        }

        return await query.ToListAsync();
    }

    public IEnumerable<CustomerVisitDetail> GetWaitingListBySP(Guid sectionId)
    {
        string? result = _context.Database.SqlQueryRaw<string>($"select get_waiting_list_by_section('{sectionId}')").AsEnumerable().FirstOrDefault();

        if (string.IsNullOrEmpty(result))
        {
            return new List<CustomerVisitDetail>();
        }

        List<CustomerVisitDetail> customerVisitDetails = JsonSerializer.Deserialize<List<CustomerVisitDetail>>(result) ?? new List<CustomerVisitDetail>();

        return customerVisitDetails;
    }

    public async Task AddAsync(CustomerVisitDetail customerVisitDetail)
    {
        await _context.CustomerVisitDetails.AddAsync(customerVisitDetail);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CustomerVisitDetail customerVisitDetail)
    {
        _context.CustomerVisitDetails.Update(customerVisitDetail);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(CustomerVisitDetail customerVisitDetail)
    {
        _context.CustomerVisitDetails.Remove(customerVisitDetail);
        await _context.SaveChangesAsync();
    }

    public async Task<string> RemoveBySP(Guid id)
    {
        var tokenIdParam = new Npgsql.NpgsqlParameter("@token_id", id);
        var messageParam = new Npgsql.NpgsqlParameter("@message", string.Empty);
       var rowsAffected = await _context.Database.ExecuteSqlRawAsync($"call delete_waiting_token(@token_id, @message)", tokenIdParam, messageParam);
        

        string message = messageParam.Value?.ToString() ?? string.Empty;
        
        Console.WriteLine(message);
        return message;
    }

}
