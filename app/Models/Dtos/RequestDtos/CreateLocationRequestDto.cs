namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class CreateLocationRequestDto
{
    public string LocationCode { get; set; } = string.Empty;
    public string? Name { get; set; }
}