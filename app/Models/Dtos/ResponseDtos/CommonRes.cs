namespace TawakalApi.app.Models.Dtos.ResponseDtos;

public class CommonRes
{
    public bool Success { get; set; }
    public int? Code { get; set; }
    public string? Message { get; set; } = string.Empty;

    public object? Data { get; set; }

}