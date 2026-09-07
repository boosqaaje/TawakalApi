namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class PartnerCancelTransactionRequestDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}