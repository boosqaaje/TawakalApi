using Microsoft.AspNetCore.Identity;
using TawakalApi.app.Data;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Models.Entities;
using Microsoft.EntityFrameworkCore;
using TawakalApi.app.Services.Util;
using TawakalApi.app.Utils.Constants;
using TawakalApi.app.Extensions;

namespace TawakalApi.app.Services.Partner;

public class PartnerService(
    AppDbContext dbContext,
    IHttpContextAccessor contextAccessor
) : IPartnerService
{

    private readonly AppDbContext _dbContext = dbContext;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;


    // Get the partner username from the HTTP context claims
    private string? GetUserEmail()
    {
        return _contextAccessor.HttpContext?.User?.GetUserEmail();
    }

    public async Task<CommonRes> CreatePartnerAsync(CreatePartnerRequestDto? dto)
    {

        // 1. Run all validations through a dedicated helper method
        var validationResult = await ValidatePartnerInputAsync(dto);

        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Success = false,
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }



        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = SystemMessages.Partner.PartnerCreated,
            ClientSecretRes = validationResult.Data
        };
    }


    private async Task<DataResult<ClientSecretResDto>> ValidatePartnerInputAsync(CreatePartnerRequestDto? dto)
    {
        if (dto == null)
        {
            return DataResult<ClientSecretResDto>.Error("Request body cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(dto.PartnerUsername))
        {
            return DataResult<ClientSecretResDto>.Error("Partner username cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(dto.PartnerName))
        {
            return DataResult<ClientSecretResDto>.Error("Partner name cannot be null or empty.");
        }


        // Check if partner username already exists in the database
        bool usernameExists = await _dbContext.PartnerEntities
        .AnyAsync(p => p.PartnerUserName == dto.PartnerUsername);

        if (usernameExists)
        {
            return DataResult<ClientSecretResDto>.Error("Username not available! please try another one.");
        }


        // 2. Generate the unique Client ID, plaintext secret (shown once!), and the hashed secret
        var (clientId, rawSecret, hashedSecret) = PartnerCredentialGenerator.GenerateNewPartnerCredentials();

        var user = await _dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == GetUserEmail());

        // 3. Map to your Partner database model
        var newPartner = new PartnerEntity
        {
            PartnerUserName = dto!.PartnerUsername,
            PartnerName = dto.PartnerName,
            ClientId = clientId,
            CurrentSecretHash = hashedSecret,
            CurrentSecretCreatedAt = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = user!.Id
        };



        // Save the partner entity to the database
        _dbContext.PartnerEntities.Add(newPartner);
        await _dbContext.SaveChangesAsync();

        var res = new ClientSecretResDto
        {
            PartnerName = dto.PartnerName!,
            ClientId = clientId,
            ClientSecret = rawSecret // Return the plaintext secret only once
        };

        return DataResult<ClientSecretResDto>.Ok(res);
    }


    public async Task<bool> IsValidPartnerAsync(string? clientId, string? clientSecret)
    {
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            return false;

        // 1. Fetch partner from database asynchronously
        var partner = await _dbContext.PartnerEntities
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
                return true; // Match found!
            }
        }

        // Neither hash matched
        return false;
    }
}