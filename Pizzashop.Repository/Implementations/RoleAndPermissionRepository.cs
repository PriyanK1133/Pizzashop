using Microsoft.EntityFrameworkCore;
using Pizzashop.Entity.Data;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Repository.Implementations;

public class RoleAndPermissionRepository : IRoleAndPermissionRepository
{
    private readonly PizzashopContext _context;

    public RoleAndPermissionRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RolesAndPermission>> GetPermissionsForRoleAsync(Guid roleId)
    {
        return await _context.RolesAndPermissions.Where(a => a.RoleId == roleId).OrderBy(a => a.Permission.Preference).ToListAsync();
    }

    public async Task<IEnumerable<RolesAndPermission>> GetPermissionsForRoleByNameAsync(string roleName)
    {
        return await _context.RolesAndPermissions.Include(a => a.Role).Include(a => a.Permission).Where(a => a.Role.Name == roleName).OrderBy(a => a.Permission.Preference).ToListAsync();
    }

    public async Task UpdateAsync(IEnumerable<RolesAndPermission> updatedPermissions, Guid roleId)
    {
        var existingPermissions = _context.RolesAndPermissions.Where(rp => rp.RoleId == roleId);

        foreach (var permission in existingPermissions)
        {
            if (updatedPermissions.FirstOrDefault(up => up.PermissionId == permission.PermissionId) == null)
            {
                _context.RolesAndPermissions.Remove(permission);
            }
        }

        foreach (var permission in updatedPermissions)
        {
            var existingPermission = existingPermissions.FirstOrDefault(ep => ep.PermissionId == permission.PermissionId);
            if (existingPermission != null)
            {
                existingPermission.CanDelete = permission.CanDelete;
                existingPermission.CanEdit = permission.CanEdit;
                existingPermission.CanView = permission.CanView;
                existingPermission.UpdatedAt = permission.UpdatedAt;
                existingPermission.UpdatedBy = permission.UpdatedBy;
            }
            else
            {
                await _context.RolesAndPermissions.AddAsync(permission);
            }
        }
        await _context.SaveChangesAsync();

    }

    public async Task<RolesAndPermission?> GetPermissionsForControllerAndRoleAsync(string controller, string roleName)
    {
        return await _context.RolesAndPermissions.Include(a => a.Role).Include(a => a.Permission).SingleOrDefaultAsync(a => a.Role.Name == roleName && a.Permission.Name.ToLower() == controller.ToLower());
    }
}
