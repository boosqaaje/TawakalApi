using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Partner;
using TawakalApi.app.Services.PasswordManagement;
using TawakalApi.app.Services.PortalAuth;

namespace TawakalApi.app.Controllers.Portal;


[ApiController]
[Route("portal/")]
[Authorize(AuthenticationSchemes = "PortalScheme")]
public class PortalController(
    IPartnerService pService,
    IPortalAuthService portalAuthService,
    IPasswordManagerService passwordService
) : BaseApiController
{
    private readonly IPartnerService _pService = pService;

    private readonly IPortalAuthService _portalAuthService = portalAuthService;
    private readonly IPasswordManagerService _passwordService = passwordService;



    [HttpPost("users/create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto? dto)
    {
        return CreatedCommonResponse(await _portalAuthService.RegisterAsync(dto));
    }

    [HttpPost("users/login")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLogin([FromBody] LoginRequestDto? dto)
    {
        return LoginResponse(await _portalAuthService.LoginAsync(dto));
    }

    [HttpPost("partners/create")]
    public async Task<IActionResult> CreatePartner(
        [FromBody] CreatePartnerRequestDto? dto
    )
    {
        return CreatedCommonResponse(await _pService.CreatePartnerAsync(dto));
    }

    [HttpPost("partners/login")]
    [AllowAnonymous]
    public async Task<IActionResult> PartnerLogin(
        [FromBody] LoginRequestDto? dto
    )
    {
        return LoginResponse(await _portalAuthService.LoginAsync(dto, isPartner: true));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto? dto
    )
    {
        return LoginResponse(await _portalAuthService.ChangePasswordAsync(dto));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestDto? dto
    )
    {
        return LoginResponse(await _passwordService.ResetPassword(dto));
    }
}