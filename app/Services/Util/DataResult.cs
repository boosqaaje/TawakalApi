namespace TawakalApi.app.Services.Util;

public class DataResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int? Code { get; set; }

    public static DataResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static DataResult<T> Error(string errorMessage) => new() { Success = false, Message = errorMessage };
    public static DataResult<T> Error(int code, string errorMessage) => new()
    { Success = false, Code = code, Message = errorMessage };
}