using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace TawakalApi.app.Services.Util;

public class PartnerCredentialGenerator
{
    private static readonly PasswordHasher<string> _hasher = new();

    public static (string clientId, string rawSecret, string hashedSecret) GenerateNewPartnerCredentials()
    {
        // 1. Generate a unique Client ID (using a clean prefix + UUID)
        string clientId = $"T_A_partner{Guid.NewGuid():N}";

        // 2. Generate a high-entropy cryptographically secure raw secret (e.g., 32 bytes)
        byte[] secretBytes = RandomNumberGenerator.GetBytes(32);
        string rawSecret = $"sec_{Convert.ToBase64String(secretBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=')}"; // URL-safe base64 string


        // 3. Hash the secret so your database never stores it in plain text
        string hashedSecret = _hasher.HashPassword(clientId, rawSecret);

        return (clientId, rawSecret, hashedSecret);
    }




//     private bool IsValidPartner(string? clientId, string? clientSecret)
// {
//     if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
//         return false;

//     // 1. Fetch the partner record from your database by clientId
//     var partnerRecord = _dbContext.Partners.FirstOrDefault(p => p.ClientId == clientId);
//     if (partnerRecord == null) return false;

//     // 2. Verify the incoming plaintext secret against the stored hash
//     var hasher = new PasswordHasher<string>();
//     var result = hasher.VerifyHashedPassword(clientId, partnerRecord.ClientSecretHash, clientSecret);

//     // Returns true if the password matches the hash
//     return result == PasswordVerificationResult.Success;
// }
}