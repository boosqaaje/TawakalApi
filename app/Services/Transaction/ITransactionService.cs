using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Services.Transaction;
public interface ITransactionService
{
    Task<CommonRes> InsertTransactionAsync(TransactionRequestDto? dto);
    Task<CommonRes> GetAll();
    Task<CommonRes> GetTransactionStatusAsync(string reference);
    Task<CommonRes> CancelTransactionAsync(string reference);
}