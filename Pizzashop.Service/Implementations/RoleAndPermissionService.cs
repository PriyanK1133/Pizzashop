using Pizzashop.Entity.Constants;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class RoleAndPermissionService : IRoleAndPermissionService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleAndPermissionRepository _roleAndPermissionRepository;

    public RoleAndPermissionService(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IRoleAndPermissionRepository roleAndPermissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _roleAndPermissionRepository = roleAndPermissionRepository;
    }

    public async Task<Response<IEnumerable<RoleVM>>> GetAllRolesAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var roles = await _roleRepository.GetAllAsync();
            var rolelist = roles.Select(r => new RoleVM()
            {
                Id = r.Id,
                Name = r.Name
            });
            return Response<IEnumerable<RoleVM>>.SuccessResponse(rolelist, "Roles "+MessageConstants.GetMessage);
        });

    }

    public async Task<Response<IEnumerable<PermissionVM>>> GetAllPermissionsAsync()
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var permissions = await _permissionRepository.GetAllAsync();
            var permissionslist = permissions.Select(p => new PermissionVM()
            {
                Id = p.Id,
                Name = p.Name
            });
            return Response<IEnumerable<PermissionVM>>.SuccessResponse(permissionslist, "Permissions "+MessageConstants.GetMessage);
        });
    }

    public async Task<Response<RoleAndPermissionVM>> GetPermissionsForRoleAsync(Guid roleId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            if (roleId.Equals(Guid.Empty))
            {
                return Response<RoleAndPermissionVM>.FailureResponse("User "+MessageConstants.NotFoundMessage);
            }
            IEnumerable<Permission> permissionslist = await _permissionRepository.GetAllAsync();
            IEnumerable<RolesAndPermission> permissionsForRole = await _roleAndPermissionRepository.GetPermissionsForRoleAsync(roleId);

            List<PermissionVM> permissions = permissionslist
                                                    .GroupJoin(permissionsForRole,
                                                    p => p.Id,
                                                    rp => rp.PermissionId,
                                                    (p, rp) => new PermissionVM()
                                                    {
                                                        Id = p.Id,
                                                        Name = p.Name,
                                                        CanView = rp.Select(t => t.CanView).FirstOrDefault(),
                                                        CanEdit = rp.Select(t => t.CanEdit).FirstOrDefault(),
                                                        CanDelete = rp.Select(t => t.CanDelete).FirstOrDefault(),
                                                    }).ToList();

            RoleAndPermissionVM roleAndPermissionVM = new()
            {
                RoleId = roleId,
                Permissions = permissions
            };

            return Response<RoleAndPermissionVM>.SuccessResponse(roleAndPermissionVM, "Permission "+MessageConstants.GetMessage);
        });
    }

    public async Task<Response<RoleAndPermissionVM>> UpdateAsync(RoleAndPermissionVM model, Guid updatorId)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            var roleId = model.RoleId;
            var permissions = model.Permissions;
            IEnumerable<RolesAndPermission> updatedPermissions = permissions.Where(up => up.CanDelete || up.CanEdit || up.CanView).Select(p => new RolesAndPermission()
            {
                RoleId = roleId,
                PermissionId = p.Id,
                CanDelete = p.CanDelete,
                CanEdit = p.CanEdit,
                CanView = p.CanView,
                CreatedBy = updatorId,
                CreatedAt = DateTime.Now,
                UpdatedBy = updatorId,
                UpdatedAt = DateTime.Now
            });

            await _roleAndPermissionRepository.UpdateAsync(updatedPermissions, roleId);

            return Response<RoleAndPermissionVM>.SuccessResponse(model, "Permissions "+MessageConstants.EditMessage);
        });
    }

    public async Task<Response<PermissionVM?>> GetPermissionsForControllerAndRoleAsync(string controller, string roleName)
    {
        return await ExceptionHandler.HandleExceptionsAsync(async () =>
        {
            RolesAndPermission? controllerPermission = await _roleAndPermissionRepository.GetPermissionsForControllerAndRoleAsync(controller, roleName);
            if (controllerPermission == null)
            {
                return Response<PermissionVM?>.FailureResponse("Permission "+MessageConstants.NotFoundMessage);
            }
            PermissionVM permissions = new()
            {
                Id = controllerPermission.Permission.Id,
                Name = controllerPermission.Permission.Name,
                CanView = controllerPermission.CanView,
                CanEdit = controllerPermission.CanEdit,
                CanDelete = controllerPermission.CanDelete
            };

            return Response<PermissionVM?>.SuccessResponse(permissions, "Permissions "+MessageConstants.GetMessage);
        });
    }
}
