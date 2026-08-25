namespace TawakalApi.app.Services.Token;

public interface ITokenService
{
    Task<string> GetToken(string clientId);
}