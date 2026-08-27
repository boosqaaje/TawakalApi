using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Util;
using TawakalApi.app.Utils.Constants;
using TawakalApi.app.Extensions;
using TawakalApi.app.Data;
using TawakalApi.app.Models.Dtos.RequestDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TawakalApi.app.Services.PasswordManagement;

public class PasswordManagerService(
    IHttpContextAccessor contextAccessor,
    AppDbContext dbContext
) : IPasswordManagerService
{


    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly AppDbContext _dbContext = dbContext;
    private static readonly PasswordHasher<string> _passwordHasher = new();

    private string? GetUserRoleFromToken()
    {
        return _contextAccessor.HttpContext?.User?.GetUserRoleFromToken();
    }


    public async Task<CommonRes> ResetPassword(ResetPasswordRequestDto? dto)
    {
        var validationResult = await ValidateResetPassword(dto);

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
            Message = "Password resetted successfully."
        };
    }

    private async Task<DataResult<bool>> ValidateResetPassword(ResetPasswordRequestDto? dto)
    {

        var role = GetUserRoleFromToken()!;

        if (role.Equals(SystemRoles.Partner))
        {
            return DataResult<bool>.Error(
             SystemCodes.Auth.AccessDenied,
             SystemMessages.Auth.AccessDeniedMsg
         );
        }


        if (dto is null)
        {
            return DataResult<bool>.Error(
                SystemCodes.BodyNull,
                SystemMessages.BodyNull
            );
        }


        if (string.IsNullOrWhiteSpace(dto.UserType))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                "User type is required"
            );
        }

        if (string.IsNullOrWhiteSpace(dto.UserId))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                "User id is required"
            );
        }

        if (string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                "New password is required"
            );
        }


        var hashedPassword = _passwordHasher.HashPassword(dto.UserId, dto.NewPassword);
        if (dto.UserType.Equals(SystemRoles.Partner))
        {
            var partner = await _dbContext.PartnerEntities
            .FirstOrDefaultAsync(p => p.PartnerUserName == dto.UserId);

            if (partner is null)
            {
                return DataResult<bool>.Error(
                    SystemCodes.NotFound,
                    "Partner username not found."
                );
            }

            partner.PasswordHash = hashedPassword;
            partner.MustChangePassword = true;
            await _dbContext.SaveChangesAsync();
        }
        else if (dto.UserType.Equals(SystemRoles.User))
        {
            var user = await _dbContext.PortalUsers
             .FirstOrDefaultAsync(p => p.Email == dto.UserId);

            if (user is null)
            {
                return DataResult<bool>.Error(
                    SystemCodes.NotFound,
                    "User with this email is not found."
                );
            }

            user.PasswordHash = hashedPassword;
            user.MustChangePassword = true;
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            return DataResult<bool>.Error(
                  SystemCodes.NotFound,
                  "User not found"
              );
        }



        return DataResult<bool>.Ok(true);
    }
}