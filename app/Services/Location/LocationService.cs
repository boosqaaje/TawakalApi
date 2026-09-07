using TawakalApi.app.Data;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Models.Entities;
using TawakalApi.app.Utils.Constants;

namespace TawakalApi.app.Services.Location;

public class LocationService(
    AppDbContext dbContext
) : ILocationService
{
    public async Task<CommonRes> CreateLocationAsync(CreateLocationRequestDto? dto)
    {

        var locationEntity = new LocationEntity
        {
            LocationCode = dto?.LocationCode ?? string.Empty,
            Name = dto?.Name ?? string.Empty
        };

        // Save the partner entity to the database
        dbContext.Locations.Add(locationEntity);
        await dbContext.SaveChangesAsync();

        var res = new CommonRes
        {
            Success = true,
            Code = SystemCodes.Success,
            Message = "Location created successfully",

        };

        return res;
    }
}