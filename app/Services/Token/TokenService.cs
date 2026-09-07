using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TawakalApi.app.Data;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Models.Entities;
using TawakalApi.app.Utils.Constants;

namespace TawakalApi.app.Services.Token;

public class TokenService(
    AppDbContext dbContext,
    IConfiguration config
) : ITokenService
{

    public async Task<CommonRes> GetPartnerToken(TokenRequestDto? dto)
    {
        if (dto is null)
        {
            return new CommonRes
            {
                Code = SystemCodes.BodyNull,
                Message = SystemMessages.BodyNull,
            };
        }

        string? grantType = dto.GrantType;
        string? clientId = dto.ClientId;
        string? clientSecret = dto.ClientSecret;

        if (grantType != "client_credentials")
        {
            return new CommonRes
            {
                Code = SystemCodes.Auth.InvalidGrantType,
                Message = SystemMessages.Auth.InvalidGrantType
            };
        }


        if (!await IsValidPartnerAsync(clientId, clientSecret))
        {
            return new CommonRes
            {
                Code = SystemCodes.Auth.InvalidClientSecret,
                Message = SystemMessages.Auth.InvalidClientSecretMsg
            };
        }


        // // Generate a JWT token
        var accessToken = await GenerateJwtTokenForPartner(clientId!);


        var tokenResponse = new TokenResponseDto
        {
            access_token = accessToken,
            token_type = "Bearer",
            expires_in = 3600 // Token expiration time in seconds (1 hour)
        };


        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = "Token successfully generated.",
            Data = tokenResponse
        };
    }

    public string GenerateJwtTokenForPortal(string username, string role)
    {
        var secretKey = Encoding.UTF8.GetBytes(config["PortalJwt:Key"]!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new(SystemMessages.UserID, username),
            new(SystemMessages.UserRole, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = config["PortalJwt:Issuer"],
            Audience = config["PortalJwt:Audience"],
            SigningCredentials = credentials
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<string> GenerateJwtTokenForPartner(string clientId)
    {

        // 1. Fetch the partner details (like PartnerName) from the database
        var partner = await dbContext.PartnerEntities
        .FirstOrDefaultAsync(p => p.ClientId == clientId);




        // 2. Generate the token contianing the PartnerName
        return GenerateJwtToken(partner!.PartnerUserName, partner.PartnerName);
    }


    private string GenerateJwtToken(string partnerUsername, string partnerName)
    {
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, partnerUsername),
            new Claim("partner_username", partnerUsername),
            new Claim("partner_name", partnerName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> IsValidPartnerAsync(string? clientId, string? clientSecret)
    {
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            return false;

        // 1. Fetch partner from database asynchronously
        var partner = await dbContext.PartnerEntities
            .FirstOrDefaultAsync(p => p.ClientId == clientId);

        if (partner == null || !partner.IsActive) return false;

        var hasher = new PasswordHasher<string>();

        // 2. Check against the CURRENT secret hash
        var currentResult = hasher.VerifyHashedPassword(clientId, partner.CurrentSecretHash, clientSecret);
        if (currentResult == PasswordVerificationResult.Success)
        {
            return true; // Match found!
        }

        // 3. If not matched, check against the NEXT (overlapping) secret hash (if it exists)
        if (!string.IsNullOrEmpty(partner.NextSecretHash))
        {
            var nextResult = hasher.VerifyHashedPassword(clientId, partner.NextSecretHash, clientSecret);
            if (nextResult == PasswordVerificationResult.Success)
            {
                await PromoteNextSecretToCurrent(partner);
                return true; // Match found!
            }
        }

        // Neither hash matched
        return false;
    }


    private async Task PromoteNextSecretToCurrent(PartnerEntity partner)
    {
        partner.CurrentSecretHash = partner.NextSecretHash!;
        partner.CurrentSecretCreatedAt = partner.NextSecretCreatedAt ?? DateTime.UtcNow;
        partner.NextSecretHash = null;
        partner.NextSecretCreatedAt = null;
        await dbContext.SaveChangesAsync();
    }

}