namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class PortalAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}