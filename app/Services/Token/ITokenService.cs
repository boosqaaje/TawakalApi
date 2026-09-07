using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Services.Token;

public interface ITokenService
{
    Task<CommonRes> GetPartnerToken(TokenRequestDto? dto);
    Task<string> GenerateJwtTokenForPartner(string clientId);
    string GenerateJwtTokenForPortal(string username, string role);
    Task<bool> IsValidPartnerAsync(string? clientId, string? clientSecret);
}