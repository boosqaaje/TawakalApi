using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Partner;
using TawakalApi.app.Services.PortalAuth;

namespace TawakalApi.app.Controllers.Portal;


[ApiController]
[Route("portal/")]
[Authorize(AuthenticationSchemes = "PortalScheme")]
public class PortalController(
    IPartnerService pService,
    IPortalAuthService portalAuthService
) : BaseApiController
{
    private readonly IPartnerService _pService = pService;

    private readonly IPortalAuthService _portalAuthService = portalAuthService;


    [HttpPost("users/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto? dto)
    {
        return LoginResponse(await _portalAuthService.LoginAsync(dto));
    }

    [HttpPost("users/create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDto? dto)
    {
        return CreatedCommonResponse(await _portalAuthService.RegisterAsync(dto));
    }


    [HttpPost("partners/create")]
    public async Task<IActionResult> CreatePartner(
        [FromBody] CreatePartnerRequestDto? dto
    )
    {
        return CreatedCommonResponse(await _pService.CreatePartnerAsync(dto));
    }
}