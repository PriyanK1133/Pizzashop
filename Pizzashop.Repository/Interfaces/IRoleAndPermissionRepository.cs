using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IRoleAndPermissionRepository
{
    Task<IEnumerable<RolesAndPermission>> GetPermissionsForRoleAsync(Guid roleId);
    Task<IEnumerable<RolesAndPermission>> GetPermissionsForRoleByNameAsync(string roleName);
    Task UpdateAsync(IEnumerable<RolesAndPermission> updatedPermissions, Guid roleId);
    Task<RolesAndPermission?> GetPermissionsForControllerAndRoleAsync(string controller, string roleName);
}
