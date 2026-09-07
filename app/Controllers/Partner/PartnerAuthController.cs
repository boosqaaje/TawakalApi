using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Partner;
using TawakalApi.app.Services.Token;

namespace TawakalApi.app.Controllers.Partner;

[ApiController]
[Route("partner/auth")]
[AllowAnonymous]
public class PartnerAuthController(
    ITokenService tokenService
    ) : BaseApiController
{

    [HttpPost("token")]
    public async Task<IActionResult> GetToken(
        [FromForm] TokenRequestDto dto
    )
    {
        // Return token response
        return AuthResponse(await tokenService.GetPartnerToken(dto));
    }
}