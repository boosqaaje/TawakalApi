namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class ResetPasswordRequestDto
{
    public string UserType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}