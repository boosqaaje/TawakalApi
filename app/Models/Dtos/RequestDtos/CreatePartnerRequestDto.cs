namespace TawakalApi.app.Models.Dtos.RequestDtos;

public class CreatePartnerRequestDto
{
    public string LocationCode { get; set; } = string.Empty;
    public string PartnerUserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PartnerEmail { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
}