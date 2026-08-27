namespace TawakalApi.app.Services.CurrentUser;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserRole { get; }
    string? PartnerUsername { get; }
}