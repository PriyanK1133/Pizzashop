using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly PizzashopContext _context;

    public PermissionRepository(PizzashopContext context){
        _context = context;
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task<IEnumerable<Permission>> GetPermissionsForRoleAsync(Guid roleId){
        return await _context.Permissions.Include(p => p.RolesAndPermissions.Where(rp => rp.RoleId == roleId)).OrderBy(r => r.Id).ToListAsync();
    }

}
