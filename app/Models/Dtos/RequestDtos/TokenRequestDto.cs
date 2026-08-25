namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class TokenRequestDto {
    public string? grant_type { get; set; } = string.Empty;
    public string? client_id { get; set; } = string.Empty;
    public string? client_secret { get; set; } = string.Empty;
}