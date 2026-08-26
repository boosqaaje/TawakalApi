namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public bool MustChangePassword { get; set; } = false;
}