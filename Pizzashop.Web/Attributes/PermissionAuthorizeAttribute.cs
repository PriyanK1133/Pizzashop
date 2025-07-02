using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Pizzashop.Entity.Constants;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissionAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permissionType;

    public PermissionAuthorizeAttribute(string permissionType)
    {
        _permissionType = permissionType;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {

        IRoleAndPermissionService roleAndPermissionService = context.HttpContext.RequestServices.GetRequiredService<IRoleAndPermissionService>();
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        bool isLogin = context.HttpContext.User.Identity?.IsAuthenticated ?? false;
        if (!isLogin)
        {
            // not logged in or role not authorized
            context.Result = new ChallengeResult();
            return;
        }

        string? roleName = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        string? controller = context.RouteData.Values["controller"]?.ToString();

        if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(controller))
        {
            context.HttpContext.Response.StatusCode = 404;
            context.Result = new ForbidResult();
            return;
        }
        Response<PermissionVM?> permissionsResponse = await roleAndPermissionService.GetPermissionsForControllerAndRoleAsync(controller, roleName);
        if (!permissionsResponse.Success)
        {
            context.HttpContext.Response.StatusCode = 404;
            context.Result = new ForbidResult();
            return;
        }
        PermissionVM newPermission = permissionsResponse.Data!;
        SessionUtils.SetPermission(context.HttpContext, newPermission);
        PermissionVM? currentPermission = newPermission;

        bool hasPermission = false;

        switch (_permissionType)
        {
            case Constants.CanView:
                hasPermission = currentPermission!.CanView;
                break;
            case Constants.CanEdit:
                hasPermission = currentPermission!.CanEdit;
                break;
            case Constants.CanDelete:
                hasPermission = currentPermission!.CanDelete;
                break;
        }

        if (!hasPermission)
        {
            context.HttpContext.Response.StatusCode = 404;
            context.Result = new ForbidResult();
        }
    }
}

