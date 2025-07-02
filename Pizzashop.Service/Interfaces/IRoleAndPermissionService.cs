using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IRoleAndPermissionService
{
    Task<Response<IEnumerable<RoleVM>>> GetAllRolesAsync();
    Task<Response<IEnumerable<PermissionVM>>> GetAllPermissionsAsync();
    Task<Response<RoleAndPermissionVM>> GetPermissionsForRoleAsync(Guid roleId);
    Task<Response<RoleAndPermissionVM>> UpdateAsync(RoleAndPermissionVM model, Guid updatorId);
    Task<Response<PermissionVM?>> GetPermissionsForControllerAndRoleAsync(string controller, string roleName);
}
