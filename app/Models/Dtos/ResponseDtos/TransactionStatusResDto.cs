namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class TransactionStatusResDto
{
    public string Reference {get; set;} = string.Empty;
    public string Status {get; set;} = string.Empty;
    public decimal Amount {get; set;}
    public DateTime CreatedAt { get; set; }
}