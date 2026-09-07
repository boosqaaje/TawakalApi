namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class ClientSecretResDto
{
    public string PartnerName { get; set; } = string.Empty;
    public string? ClientId { get; set; }
    public string ClientSecret { get; set; } = string.Empty;
}