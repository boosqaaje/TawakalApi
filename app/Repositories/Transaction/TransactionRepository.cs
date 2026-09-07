using System.Data;
using Dapper;
using TawakalApi.app.Models.Dtos;
using TawakalApi.app.Models.Dtos.RequestDtos;
using TawakalApi.app.Models.Dtos.ResponseDtos;
using TawakalApi.app.Repositories.ProcedureRepo;

namespace TawakalApi.app.Repositories.Transaction;

public class TransactionRepository(IStoredProcedureRepository dbRepo) : ITransactionRepository
{

    private readonly IStoredProcedureRepository _dbRepo = dbRepo;

    public async Task<CommonRes?> InsertTransactionAsync(string username, PartnerTransactionRequestDto dto)
    {

        var parameters = new DynamicParameters();
        parameters.Add("@PartnerUsername", username);
        parameters.Add("@ReferenceId", dto.ReferenceId);
        parameters.Add("@Amount", dto.Amount);
        parameters.Add("@Currency", dto.Currency);
        parameters.Add("@ServiceCode", dto.ServiceCode);
        parameters.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ResponseMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);

        return await _dbRepo.ExecuteWithCommonResAsync("sp_InsertTransaction", parameters);
    }
}