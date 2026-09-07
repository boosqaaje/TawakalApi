using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Services.PasswordManagement;

public interface IPasswordManagerService {
    
    Task<CommonRes> ResetPassword(ResetPasswordRequestDto? dto);
}