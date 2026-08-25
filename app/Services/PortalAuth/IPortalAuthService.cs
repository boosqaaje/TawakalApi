using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Services.PortalAuth;

public interface IPortalAuthService
{
    Task<CommonRes> RegisterAsync(RegisterRequestDto? dto);
    Task<CommonRes> LoginAsync(LoginRequestDto? dto);
}