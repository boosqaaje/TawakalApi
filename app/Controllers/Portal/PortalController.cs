using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Location;
using TawakalApi.app.Services.Partner;
using TawakalApi.app.Services.PasswordManagement;
using TawakalApi.app.Services.PortalAuth;
using TawakalApi.app.Services.SecretManagement;
using TawakalApi.app.Services.Transaction;

namespace TawakalApi.app.Controllers.Portal;


[ApiController]
[Route("portal")]
[Authorize(AuthenticationSchemes = "PortalScheme")]
public class PortalController(
    IPartnerService pService,
    IPortalAuthService portalAuthService,
    IPasswordManagerService passwordManager,
    ITransactionService tService,
    ISecretManagerService secretManager,
    ILocationService locationService
) : BaseApiController
{

    // FOR PORTAL
    private readonly ITransactionService _tService = tService;

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto? dto
    )
    {
        return AuthResponse(await portalAuthService.ChangePasswordAsync(dto));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequestDto? dto
    )
    {
        return AuthResponse(await passwordManager.ResetPassword(dto));
    }
    [HttpPost("create-location")]
    public async Task<IActionResult> CreateLocation(
        [FromBody] CreateLocationRequestDto? dto
    )
    {
        return OkCommonResponse(await locationService.CreateLocationAsync(dto));
    }

    // FOR USERS
    [HttpPost("users/create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto? dto)
    {
        return CreatedCommonResponse(await portalAuthService.RegisterAsync(dto));
    }

    [HttpPost("users/login")]
    [AllowAnonymous]
    public async Task<IActionResult> UserLogin([FromBody] LoginRequestDto? dto)
    {
        return AuthResponse(await portalAuthService.LoginAsync(dto));
    }

    // FOR PARTNERS
    [HttpPost("partners/create")]
    public async Task<IActionResult> CreatePartner(
        [FromBody] CreatePartnerRequestDto? dto
    )
    {
        return CreatedCommonResponse(await pService.CreatePartnerAsync(dto));
    }

    [HttpPost("partners/login")]
    [AllowAnonymous]
    public async Task<IActionResult> PartnerLogin(
        [FromBody] LoginRequestDto? dto
    )
    {
        return AuthResponse(await portalAuthService.LoginAsync(dto, isPartner: true));
    }

    [HttpGet("partners/reset-secret")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetSecret(
        [FromBody] LoginRequestDto? dto
    )
    {
        return OkCommonResponse(await secretManager.ResetClientSecret());
    }

    [HttpGet("partners/transactions")]
    public async Task<IActionResult> GetAll()
    {
        return OkCommonResponse(await _tService.GetAll());
    }
}