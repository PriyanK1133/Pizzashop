using Pizzashop.Service.Interfaces;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Middlewares;
public class CreateAccessTokenMiddleware
{
    private readonly RequestDelegate _next;

    public CreateAccessTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IJwtService jwtService)
    {
        try
        {
            var accessToken = CookieUtils.GetAccessToken(context.Request);
            var refreshToken = CookieUtils.GetRefreshToken(context.Request);

            if (accessToken != null)
            {
                var tokenPrinciple = jwtService.ValidateToken(accessToken);
                if (tokenPrinciple != null)
                {
                    // Token is valid, proceed to the next middleware
                    await _next(context);
                    return;
                }
            }

            if (refreshToken != null)
            {
                var tokenPrinciple = jwtService.ValidateToken(refreshToken);
                if (tokenPrinciple != null)
                {
                    // Refresh token is valid, generate a new access token
                    var accountId = Guid.Parse(tokenPrinciple.Claims.First(c => c.Type == "AccountId").Value);
                    var newAccessToken = await jwtService.GenerateAccessTokenAsync(accountId);
                    CookieUtils.SaveAccessToken(context.Response, newAccessToken);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Invalid token: {ex.Message}");
        }

        // Proceed to the next middleware
        await _next(context);
    }
}

public static class CreateAccessTokenMiddlewareExtension
{
    public static IApplicationBuilder UseCreateAccessToken(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CreateAccessTokenMiddleware>();
    }
}