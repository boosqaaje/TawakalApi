namespace TawakalApi.app.Models.Dtos.RequestDtos;


public class LoginRequestDto
{
    public string? Email { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}