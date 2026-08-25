using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TawakalApi.app.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetPartnerUsername(this ClaimsPrincipal user)
    {
        return user.FindFirst("partner_username")?.Value 
               ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
               ?? string.Empty;
    }
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst("user_email")?.Value 
               ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
               ?? string.Empty;
    }
}