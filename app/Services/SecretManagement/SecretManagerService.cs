using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TawakalApi.app.Data;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.CurrentUser;
using TawakalApi.app.Services.Token;
using TawakalApi.app.Utils.Constants;

namespace TawakalApi.app.Services.SecretManagement;


public class SecretManagerService(
    AppDbContext dbContext,
    ICurrentUserService currentUser
) : ISecretManagerService
{


    private static readonly PasswordHasher<string> _passwordHasher = new();
    public async Task<CommonRes> ResetClientSecret()
    {
        var partner = await dbContext.PartnerEntities
        .FirstOrDefaultAsync(p => p.PartnerUserName == currentUser.UserId);

        if (partner is null)
        {
            return new CommonRes
            {
                Code = SystemCodes.Partner.PartnerNotFound,
                Message = SystemMessages.Partner.PartnerNotFound
            };
        }

        string rawSecret = GenerateSecureSecret();

        // 4. Hash the secret before storing it in the database
        string hashedSecret = _passwordHasher.HashPassword(currentUser.PartnerUsername!, rawSecret);

        // 5. Implement the Overlap Window logic:
        // Move the old current secret down or assign to NextSecretHash safely
        partner.NextSecretHash = hashedSecret;
        partner.NextSecretCreatedAt = DateTime.UtcNow;

        // Save changes to database
        await dbContext.SaveChangesAsync();

        var res = new ClientSecretResDto
        {
            ClientSecret = rawSecret, // Return the plaintext secret only once,
            PartnerName = partner.PartnerName,
        };


        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = "Client secret rotated successfully. Save this secret immediately; it will not be shown again.",
            Data = res
        };
    }


    private static string GenerateSecureSecret()
    {
        // 2. Generate a high-entropy cryptographically secure raw secret (e.g., 32 bytes)
        byte[] secretBytes = RandomNumberGenerator.GetBytes(32);
        string rawSecret = $"sec_{Convert.ToBase64String(secretBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=')}"; // URL-safe base64 string

        return rawSecret;
    }
}