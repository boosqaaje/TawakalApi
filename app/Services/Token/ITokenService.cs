namespace TawakalApi.app.Services.Token;

public interface ITokenService
{
    Task<string> GetToken(string clientId);
    string GenerateJwtTokenForPortal(string username, string role);
}