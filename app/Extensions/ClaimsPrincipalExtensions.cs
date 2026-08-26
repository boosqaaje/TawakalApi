using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TawakalApi.app.Utils.Constants;

namespace TawakalApi.app.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetPartnerUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst("partner_username")?.Value
               ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? string.Empty;
    }
    public static string GetUserIDFromToken(this ClaimsPrincipal user)
    {
        return user.FindFirst(SystemMessages.UserID)?.Value
               ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? string.Empty;
    }
    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(SystemMessages.UserRole)?.Value
               ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
               ?? string.Empty;
    }
}