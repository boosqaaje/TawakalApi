using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TawakalApi.app.Data;
using TawakalApi.app.Utils.Constants;

namespace TawakalApi.app.Services.Token;

public class TokenService(
    AppDbContext dbContext,
    IConfiguration config
) : ITokenService
{


    public  string GenerateJwtTokenForPortal(string username, string role)
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

    public async Task<string> GetToken(string clientId)
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
}