namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class TransactionResponseDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ServiceCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}