using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Models.Entities;
using TawakalApi.app.Services.Util;

namespace TawakalApi.app.Services.Partner;

public interface IPartnerService
{
    // Task<DataResult<ClientSecretRes>> CreatePartnerAsync(CreatePartnerRequestDto? dto);
    Task<CommonRes> CreatePartnerAsync(CreatePartnerRequestDto? dto);

    Task<bool> IsValidPartnerAsync(string? clientId, string? clientSecret);
}