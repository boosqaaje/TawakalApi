using Dapper;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Repositories.ProcedureRepo;

public interface IStoredProcedureRepository
{
    Task<T?> ExecuteStoredProcedureAsync<T>(string procedureName, DynamicParameters parameters);
    Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, DynamicParameters parameters);

    Task<CommonRes> ExecuteWithCommonResAsync(string procedureName, DynamicParameters parameters);
}