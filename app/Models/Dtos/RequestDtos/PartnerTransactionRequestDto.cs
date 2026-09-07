namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class PartnerTransactionRequestDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ServiceCode { get; set; } = string.Empty;
}