using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly PizzashopContext _context;

    public RoleRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(){
        return await _context.Roles.Where(r => r.Name != Constants.SuperAdmin).OrderBy(r => r.Id).ToListAsync();
    }
}
