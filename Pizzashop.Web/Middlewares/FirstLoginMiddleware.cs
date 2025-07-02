using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Middlewares;
public class FirstLoginMiddleware
{
    private readonly RequestDelegate _next;

    public FirstLoginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                bool isFirstLogin = SessionUtils.GetIsFirstLogin(context);
                if (isFirstLogin && context.Request.Path != "/Home/ChangePassword")
                {
                    context.Response.Redirect("/Home/ChangePassword");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Invalid First Login: {ex.Message}");
        }

        await _next(context);
    }
}

public static class FirstLoginMiddlewareExtension
{
    public static IApplicationBuilder UseFirstLogin(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FirstLoginMiddleware>();
    }
}