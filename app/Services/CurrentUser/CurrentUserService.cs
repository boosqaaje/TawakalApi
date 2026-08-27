using System.Security.Claims;
using TawakalApi.app.Extensions;

namespace TawakalApi.app.Services.CurrentUser;

public class CurrentUserService(
    IHttpContextAccessor httpContextAccessor
) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;
    
    public string? UserId => _user?.GetUserIDFromToken();
     

    public string? UserRole => _user?.GetUserRoleFromToken();

    public string? PartnerUsername => _user?.GetPartnerUsername();
}