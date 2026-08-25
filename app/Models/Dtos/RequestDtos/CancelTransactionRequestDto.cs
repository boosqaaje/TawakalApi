namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class CancelTransactionRequestDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}