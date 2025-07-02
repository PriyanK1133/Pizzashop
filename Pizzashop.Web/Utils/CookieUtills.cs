using Pizzashop.Entity.Data;
using System.Text.Json;

namespace Pizzashop.Web.Utils
{
    public static class CookieUtils
    {
        public static void SaveRefreshToken(HttpResponse response, string token)
        {
            response.Cookies.Append("refreshToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMonths(1)
            });
        }
        public static void SaveAccessToken(HttpResponse response, string token)
        {
            response.Cookies.Append("accessToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            });
        }
        
        public static string? GetRefreshToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("refreshToken", out string? token);
            return token;
        }

        public static string? GetAccessToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("accessToken", out string? token);
            return token;
        }
        
        public static void SaveUserData(HttpResponse response, Account user)
        {
            string userData = JsonSerializer.Serialize(user);

            // Store user details in a cookie for 3 days
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };
            response.Cookies.Append("UserData", userData, cookieOptions);
        }

        public static void ClearCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("refreshToken");
            httpContext.Response.Cookies.Delete("accessToken");
        }
    }
}