using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Services.Location;

public interface ILocationService
{
    Task<CommonRes> CreateLocationAsync(CreateLocationRequestDto? dto);
}