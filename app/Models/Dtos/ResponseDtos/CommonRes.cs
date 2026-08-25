namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class CommonRes
{
    public bool Success { get; set; }
    public int? Code { get; set; }
    public string? Message { get; set; } = string.Empty;


    // Object properties
    public TransactionStatusResDto? TransactionStatus { get; set; }
    public PortalAuthResponseDto? PortalAuthResponse { get; set; }
    public ClientSecretResDto? ClientSecretRes { get; set; }

    public object? Data { get; set; }

}