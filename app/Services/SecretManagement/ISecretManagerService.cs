using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Services.SecretManagement;


public interface ISecretManagerService
{
    Task<CommonRes> ResetClientSecret();
}