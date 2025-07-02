using Pizzashop.Entity.Data;

namespace Pizzashop.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetPermissionsForRoleAsync(Guid roleId);
}
