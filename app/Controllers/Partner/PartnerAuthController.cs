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
    IPartnerService pService,
    ITokenService tokenService
    ) : ControllerBase
{
    private readonly IPartnerService _pService = pService;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("token")]
    public async Task<IActionResult> GetToken(
        [FromForm] TokenRequestDto requestDto
    )
    {

        string? grantType = requestDto.grant_type;
        string? clientId = requestDto.client_id;
        string? clientSecret = requestDto.client_secret;

        if (grantType != "client_credentials")
        {
            return BadRequest(new
            {
                error = "invalid_grant",
                error_description = "The grant_type must be 'client_credentials'."
            });
        }

        if (!await _pService.IsValidPartnerAsync(clientId, clientSecret))
        {
            return Unauthorized(new
            {
                error = "invalid_client",
                error_description = "Client authentication failed. Check your client_id and client_secret."
            });
        }

        // Generate a JWT token
        var accessToken = await _tokenService.GetToken(clientId!);


        var tokenResponse = new TokenResponseDto
        {
            access_token = accessToken,
            token_type = "Bearer",
            expires_in = 3600 // Token expiration time in seconds (1 hour)
        };


        // Return token response
        return Ok(tokenResponse);
    }
}