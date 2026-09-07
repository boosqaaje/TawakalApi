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

    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    private static readonly PasswordHasher<string> _passwordHasher = new();


    // Get the partner username from the HTTP context claims
    private string? GetUserEmailFromToken()
    {
        return _contextAccessor.HttpContext?.User?.GetUserIDFromToken();
    }
    private string? GetUserRoleFromToken()
    {
        return _contextAccessor.HttpContext?.User?.GetUserRoleFromToken();
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
            Data = validationResult.Data
        };
    }


    private async Task<DataResult<ClientSecretResDto>> ValidatePartnerInputAsync(CreatePartnerRequestDto? dto)
    {

        if (GetUserRoleFromToken() == SystemRoles.Partner)
        {
            return DataResult<ClientSecretResDto>.Error(
                SystemCodes.Auth.AccessDenied,
                SystemMessages.Auth.AccessDeniedMsg
            );
        }

        if (dto == null)
        {
            return DataResult<ClientSecretResDto>.Error(
                SystemCodes.BodyNull,
                SystemMessages.BodyNull
                );
        }

        if (string.IsNullOrWhiteSpace(dto.PartnerUserName))
        {
            return DataResult<ClientSecretResDto>.Error("Partner username cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return DataResult<ClientSecretResDto>.Error("Password cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(dto.PartnerEmail))
        {
            return DataResult<ClientSecretResDto>.Error("Partner email cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(dto.PartnerName))
        {
            return DataResult<ClientSecretResDto>.Error("Partner name cannot be null or empty.");
        }


        // Check if partner username already exists in the database
        bool usernameExists = await dbContext.PartnerEntities
        .AnyAsync(p => p.PartnerUserName == dto.PartnerUserName);


        // Check if partner username already exists in the database
        bool emailExists = await dbContext.PartnerEntities
        .AnyAsync(p => p.PartnerEmail == dto.PartnerEmail);

        if (usernameExists)
        {
            return DataResult<ClientSecretResDto>.Error(
                SystemCodes.Auth.UserNameExist,
                "Username not available! please try another one.");
        }

        if (emailExists)
        {
            return DataResult<ClientSecretResDto>.Error(
                SystemCodes.Auth.EmailExist,
                "Email already registered with another partner! please try another one.");
        }


        // 2. Generate the unique Client ID, plaintext secret (shown once!), and the hashed secret
        var (clientId, rawSecret, hashedSecret) = PartnerCredentialGenerator.GenerateNewPartnerCredentials();

        var user = await dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == GetUserEmailFromToken());
        var hashedPassword = _passwordHasher.HashPassword(dto.PartnerUserName, dto.Password);

        // 3. Map to your Partner database model
        var newPartner = new PartnerEntity
        {
            PartnerEmail = dto!.PartnerEmail,
            PartnerUserName = dto.PartnerUserName,
            PartnerName = dto.PartnerName,
            ClientId = clientId,
            PasswordHash = hashedPassword,
            CurrentSecretHash = hashedSecret,
            CurrentSecretCreatedAt = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = user!.Id
        };



        // Save the partner entity to the database
        dbContext.PartnerEntities.Add(newPartner);
        await dbContext.SaveChangesAsync();

        var res = new ClientSecretResDto
        {
            PartnerName = dto.PartnerName!,
            ClientId = clientId,
            ClientSecret = rawSecret // Return the plaintext secret only once
        };

        return DataResult<ClientSecretResDto>.Ok(res);
    }




}