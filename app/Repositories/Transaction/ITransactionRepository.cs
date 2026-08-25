namespace TawakalApi.app.Repositories.Transaction;

using TawakalApi.app.Models.Dtos;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;

public interface ITransactionRepository
{
    Task<CommonRes?> InsertTransactionAsync(string username, TransactionRequestDto dto);
}