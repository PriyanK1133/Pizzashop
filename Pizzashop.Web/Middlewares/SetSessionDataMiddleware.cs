using System.IdentityModel.Tokens.Jwt;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Middlewares;

public class SetSessionDataMiddleware
{
    private readonly RequestDelegate _next;

    public SetSessionDataMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService _userService, IRoleAndPermissionService _roleAndPermissionService)
    {
        var accessToken = context.Request.Cookies["accessToken"];
        if (context.User.Identity?.IsAuthenticated != true || string.IsNullOrEmpty(accessToken))
        {
            await _next(context);
            return;
        }

        var handler = new JwtSecurityTokenHandler();
        try
        {
            UserVM? user = SessionUtils.GetUser(context);
            if (handler.ReadToken(accessToken) is JwtSecurityToken jwtToken)
            {
                Guid userId = Guid.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId")!.Value);
                bool isFirstLogin = bool.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "IsFirstLogin")!.Value);

                Response<UserVM?> response = await _userService.GetByIdAsync(userId);
                if (!response.Success)
                {
                    context.Response.StatusCode = 404;
                    context.Request.Path = "/Home/NotFound";
                    await _next(context);
                    return;
                }

                user = response.Data;
                SessionUtils.SetUser(context, user!);
                SessionUtils.SetUserId(context, userId);
                SessionUtils.SetIsFirstLogin(context, isFirstLogin);
            }

            Response<RoleAndPermissionVM> permissionResponse = await _roleAndPermissionService.GetPermissionsForRoleAsync(user!.RoleId);
            if (!permissionResponse.Success)
            {
                context.Response.StatusCode = 404;
                context.Request.Path = "/Home/NotFound";
                await _next(context);
                return;
            }

            SessionUtils.SetPermissions(context, permissionResponse.Data!.Permissions);


        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Invalid token: {ex.Message}");
        }

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}

public static class SetSessionDataMiddlewareExtension
{
    public static IApplicationBuilder UseSetSessionData(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SetSessionDataMiddleware>();
    }
}