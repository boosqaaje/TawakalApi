using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Services.Util;
using TawakalApi.app.Utils.Constants;
using TawakalApi.app.Extensions;

namespace TawakalApi.app.Services.PasswordManagement;

public class PasswordManagerService(
    IHttpContextAccessor contextAccessor
) : IPasswordManagerService
{


    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    private string? GetUserRoleFromToken()
    {
        return _contextAccessor.HttpContext?.User?.GetUserRole();
    }


    public async Task<CommonRes> ResetPassword(string newPassword)
    {

        var validationResult = ValidateResetPassword(newPassword);

        if (!validationResult.Success)
        {
            return new CommonRes
            {
                Code = validationResult.Code,
                Message = validationResult.Message
            };
        }

        

      return new CommonRes {};
    }

    private DataResult<bool> ValidateResetPassword(string newPassword)
    {

        var role = GetUserRoleFromToken()!;

        if (role.Equals(SystemRoles.Partner))
        {
            return DataResult<bool>.Error(
             SystemCodes.Auth.AccessDenied,
             SystemMessages.Auth.AccessDeniedMsg
         );
        }


        if (string.IsNullOrWhiteSpace(newPassword))
        {
            return DataResult<bool>.Error(
                SystemCodes.RequiredField,
                "New password is required"
            );
        }
        return DataResult<bool>.Ok(true);
    }
}