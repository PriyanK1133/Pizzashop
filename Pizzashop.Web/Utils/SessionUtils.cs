using System.Text.Json;
using Pizzashop.Entity.ViewModel;

namespace Pizzashop.Web.Utils
{
    public static class SessionUtils
    {

        public static void SetUserId(HttpContext httpContext, Guid userId)
        {
            httpContext.Session.SetString("UserId", userId.ToString());
        }

        public static Guid? GetUserId(HttpContext httpContext)
        {
            // Check session first
            string? userId = httpContext.Session.GetString("UserId");

            // return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<Account>(userData);
            return Guid.TryParse(userId, out Guid parsedUserId) ? parsedUserId : (Guid?)null;
        }
        public static void SetIsFirstLogin(HttpContext httpContext, bool isFirstLogin)
        {
            httpContext.Session.SetString("IsFirstLogin", isFirstLogin.ToString());
        }

        public static bool GetIsFirstLogin(HttpContext httpContext)
        {
            // Check session first
            string? userId = httpContext.Session.GetString("IsFirstLogin");

            // return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<Account>(userData);
            return !bool.TryParse(userId, out bool parsedIsFirstLogin) || parsedIsFirstLogin;
        }
        public static void SetUser(HttpContext httpContext, UserVM user)
        {
            string userData = JsonSerializer.Serialize(user);
            httpContext.Session.SetString("UserData", userData);
        }

        public static UserVM? GetUser(HttpContext httpContext)
        {
            // Check session first
            string? userData = httpContext.Session.GetString("UserData");

            return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<UserVM>(userData);
        }

        public static void SetPermission(HttpContext httpContext, PermissionVM permission)
        {
            string permissionData = JsonSerializer.Serialize(permission);
            httpContext.Session.SetString("permission", permissionData);
        }

        public static PermissionVM? GetPermission(HttpContext httpContext)
        {
            // Check session first
            string? permission = httpContext.Session.GetString("permission");

            return string.IsNullOrEmpty(permission) ? null : JsonSerializer.Deserialize<PermissionVM>(permission);
        }

        public static void SetPermissions(HttpContext httpContext, List<PermissionVM> permissions)
        {
            List<string> permissionList = permissions.Where(p => p.CanView).Select(p => p.Name).ToList();
            string permissionData = JsonSerializer.Serialize(permissionList);
            httpContext.Session.SetString("controllerPermissions", permissionData);
        }

        public static List<string>? GetPermissions(HttpContext httpContext)
        {
            // Check session first
            string? permission = httpContext.Session.GetString("controllerPermissions");

            return string.IsNullOrEmpty(permission) ? null : JsonSerializer.Deserialize<List<string>>(permission);
        }
        
        public static void ClearSession(HttpContext httpContext) => httpContext.Session.Clear();
    }
}