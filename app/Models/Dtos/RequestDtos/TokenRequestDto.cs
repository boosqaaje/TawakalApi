namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class TokenRequestDto
{
    public string? GrantType { get; set; } = string.Empty;
    public string? ClientId { get; set; } = string.Empty;
    public string? ClientSecret { get; set; } = string.Empty;
}