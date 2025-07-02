using System.Security.Claims;

namespace Pizzashop.Service.Interfaces;
public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(Guid accountId);
    string GenerateAccessToken(Guid userId, string roleName, bool isFirstLogin);
    string GenerateRefreshToken(Guid accountId);
    ClaimsPrincipal? ValidateToken(string token);
}

