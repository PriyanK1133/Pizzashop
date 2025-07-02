using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly PizzashopContext _context;

    public UserRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var user = await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        return user;
    }

    public async Task<(IEnumerable<User> list, int totalRecords)> GetPagedUsersAsync(string searchString, int page, int pagesize, string sortColumn, bool isASC)
    {
        IQueryable<User> query = _context.Users.Include(i => i.Role).Where(i => !i.IsDeleted && i.Role.Name != Constants.SuperAdmin);
        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            query = query.Where(i => string.Concat(i.FirstName, " ", i.LastName).ToLower().Contains(searchString)
                                   || i.Email.ToLower().Contains(searchString)
                                   || i.Phone.ToLower().Contains(searchString)
                                   || i.Role.Name.ToLower().Contains(searchString)
                                   );
        }

        if (isASC)
        {
            query = sortColumn.ToLower() switch
            {
                "role" => query =  query.OrderBy(i => i.Role.Name),
                _ => query = query.OrderBy(i => string.Concat(i.FirstName, i.LastName))
            };
        }
        else
        {
            query = sortColumn.ToLower() switch
            {
                "role" => query = query.OrderByDescending(i => i.Role.Name),
                _ => query = query.OrderByDescending(i => string.Concat(i.FirstName, i.LastName))
            };
        }

        return (await query.Skip((page - 1) * pagesize).Take(pagesize).ToListAsync(), await query.CountAsync());
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users.AnyAsync(predicate);
    }
    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
