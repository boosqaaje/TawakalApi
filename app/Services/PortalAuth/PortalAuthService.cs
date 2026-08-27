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
using TawakalApi.app.Services.Util;
using TawakalApi.app.Utils.Constants;
using TawakalApi.app.Extensions;
using TawakalApi.app.Services.CurrentUser;
using TawakalApi.app.Services.Token;

namespace TawakalApi.app.Services.PortalAuth;

public class PortalAuthService(
    AppDbContext dbContext,
    ICurrentUserService currentUser,
    ITokenService tokenService
) : IPortalAuthService
{


    private static readonly PasswordHasher<string> _passwordHasher = new();

    public async Task<CommonRes> RegisterAsync(RegisterRequestDto? dto)
    {

        var validationResult = await ValidateRegistrationFields(dto);

        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Success = false,
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }

        var activeUser = await dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == currentUser.UserId);

        // Map all fields including the new name properties
        var user = new PortalUserEntity
        {
            FirstName = dto!.FirstName,
            MiddleName = dto.MiddleName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = dto.Role.Trim().ToUpperInvariant(),
            CreatedByUserId = activeUser!.Id,
            MustChangePassword = true
        };

        // Securely hash the password
        user.PasswordHash = _passwordHasher.HashPassword(user.Email, dto.Password);

        dbContext.PortalUsers.Add(user);
        await dbContext.SaveChangesAsync();

        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = SystemMessages.Auth.UserRegistered
        };
    }

    private async Task<DataResult<bool>> ValidateRegistrationFields(RegisterRequestDto? dto)
    {

        if (currentUser.UserRole == SystemRoles.Partner)
        {
            return DataResult<bool>.Error(
                SystemCodes.Auth.AccessDenied,
                SystemMessages.Auth.AccessDeniedMsg
            );
        }

        if (dto == null)
        {
            return DataResult<bool>.Error(
                SystemCodes.BodyNull,
                SystemMessages.BodyNull
                );
        }

        // Check if email already exists
        var existingEmail = await dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingEmail != null)
        {
            return DataResult<bool>.Error(
                SystemCodes.Auth.EmailExist,
                SystemMessages.Auth.EmailExist
            );
        }
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.FirstNameRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.LastNameRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.EmailRequired
            );
        }


        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.PasswordRequired
            );
        }

        if (string.IsNullOrWhiteSpace(dto.Role))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                SystemMessages.Auth.RoleRequired
            );
        }

        return DataResult<bool>.Ok(true);
    }



    public async Task<CommonRes> LoginAsync(LoginRequestDto? dto, bool isPartner)
    {
        // Validation
        var validationResult = await ValidateLoginFields(dto, isPartner);
        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }


        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = SystemMessages.Auth.LoginSuccess,
            Data = validationResult.Data
        };
    }

    // Validate Login
    private async Task<DataResult<LoginResponseDto>> ValidateLoginFields(LoginRequestDto? dto, bool isPartner)
    {
        if (dto is null) return DataResult<LoginResponseDto>.Error(
            SystemCodes.BodyNull,
            SystemMessages.BodyNull
        );


        if (string.IsNullOrWhiteSpace(dto.Password)) return DataResult<LoginResponseDto>.Error(
            SystemCodes.RequiredField,
            SystemMessages.Auth.PasswordRequired
        );

        var username = "";
        var role = "";
        var mustChangePassword = false;

        if (isPartner)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName))
            {
                return DataResult<LoginResponseDto>.Error(
                    SystemCodes.RequiredField,
                    SystemMessages.Auth.UserNameRequired
                );
            }
            username = dto.UserName;
            role = SystemRoles.Partner;
            var partner = await dbContext.PartnerEntities
            .FirstOrDefaultAsync(u => u.PartnerUserName == dto.UserName);

            if (partner is null)
            {
                return DataResult<LoginResponseDto>.Error(
                    SystemCodes.Auth.InvalidCredentials,
                    SystemMessages.Auth.InvalidCredentials
                );
            }

            mustChangePassword = partner.MustChangePassword;

            // Verify password
            var partnerPassword = _passwordHasher.VerifyHashedPassword(dto.UserName, partner.PasswordHash, dto.Password);
            if (partnerPassword == PasswordVerificationResult.Failed)
            {
                return DataResult<LoginResponseDto>.Error(
                    SystemCodes.Auth.InvalidCredentials,
                    SystemMessages.Auth.InvalidCredentials
                );
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return DataResult<LoginResponseDto>.Error(
                    SystemCodes.RequiredField,
                    SystemMessages.Auth.EmailRequired
                );
            }
            username = dto.Email;
            var user = await dbContext.PortalUsers
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null)
            {
                return DataResult<LoginResponseDto>.Error(
                    SystemCodes.Auth.InvalidCredentials,
                    SystemMessages.Auth.InvalidCredentials
                );
            }

            role = user.Role;
            mustChangePassword = user.MustChangePassword;

            // Verify password
            var result = _passwordHasher.VerifyHashedPassword(dto.Email, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return DataResult<LoginResponseDto>.Error(
                    SystemCodes.Auth.InvalidCredentials,
                    SystemMessages.Auth.InvalidCredentials
                );
            }

        }

        var loginRes = new LoginResponseDto
        {
            Token = tokenService.GenerateJwtTokenForPortal(username, role),
            MustChangePassword = mustChangePassword

        };

        return DataResult<LoginResponseDto>.Ok(loginRes);
    }


    public async Task<CommonRes> ChangePasswordAsync(ChangePasswordRequestDto? dto)
    {
        var validationResult = await ValidateChangePassword(dto);

        var r = validationResult.Success;

        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }

        return new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = SystemMessages.Auth.PasswordChanged
        };
    }


    private async Task<DataResult<bool>> ValidateChangePassword(ChangePasswordRequestDto? dto)
    {
        if (dto is null)
            return DataResult<bool>.Error(
                SystemCodes.BodyNull,
                SystemMessages.BodyNull
            );


        if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
        {
            return DataResult<bool>.Error(
            SystemCodes.RequiredField,
            "Current password is required"
        );
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return DataResult<bool>.Error(
            SystemCodes.RequiredField,
            "New password is required"
        );
        }

        var userId = currentUser.UserId!;

        var partner = await dbContext.PartnerEntities.FirstOrDefaultAsync(p => p.PartnerUserName == userId);
        if (partner is not null)
        {
            var partnerPassword = _passwordHasher.VerifyHashedPassword(userId, partner.PasswordHash, dto.CurrentPassword);
            if (partnerPassword == PasswordVerificationResult.Failed)
            {
                return DataResult<bool>.Error(
                    SystemCodes.Auth.InvalidCredentials,
                    SystemMessages.Auth.InvalidCredentials
                );
            }

            partner.PasswordHash = _passwordHasher.HashPassword(userId, dto.NewPassword);
            partner.MustChangePassword = false;
            await dbContext.SaveChangesAsync();
            return DataResult<bool>.Ok(true);
        }

        var user = await dbContext.PortalUsers.FirstOrDefaultAsync(u => u.Email == userId);
        if (user is null)
        {
            return DataResult<bool>.Error(
                SystemCodes.Auth.InvalidCredentials,
                SystemMessages.Auth.InvalidCredentials
            );
        }

        var userPassword = _passwordHasher.VerifyHashedPassword(userId, user.PasswordHash, dto.CurrentPassword);
        if (userPassword == PasswordVerificationResult.Failed)
        {
            return DataResult<bool>.Error(
                SystemCodes.Auth.InvalidCredentials,
                SystemMessages.Auth.InvalidCredentials
            );
        }

        user.PasswordHash = _passwordHasher.HashPassword(userId, dto.NewPassword);
        user.MustChangePassword = false;
        await dbContext.SaveChangesAsync();

        return DataResult<bool>.Ok(true);
    }
}