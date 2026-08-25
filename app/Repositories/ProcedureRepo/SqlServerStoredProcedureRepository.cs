using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TawakalApi.app.Models.Dtos.ResponseDtos;

namespace TawakalApi.app.Repositories.ProcedureRepo;

public class SqlServerStoredProcedureRepository(IConfiguration configuration) : IStoredProcedureRepository
{

    private readonly string _connectionString = configuration.GetConnectionString("DBConnection") ??
    throw new InvalidOperationException("Connection string not found.");
    public async Task<T?> ExecuteStoredProcedureAsync<T>(string procedureName, DynamicParameters parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<T>> QueryStoredProcedureAsync<T>(string procedureName, DynamicParameters parameters)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QueryAsync<T>(procedureName, parameters, commandType: CommandType.StoredProcedure);
    }




    public async Task<CommonRes> ExecuteWithCommonResAsync(string procedureName, DynamicParameters parameters)
    {
        // Ensure output parameters are always included if they aren't already
        if (!parameters.ParameterNames.Contains("@ResponseCode"))
        {
            parameters.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        }
        if (!parameters.ParameterNames.Contains("@ResponseMessage"))
        {
            parameters.Add("@ResponseMessage", dbType: DbType.String, size: 250, direction: ParameterDirection.Output);
        }

        // Execute the stored procedure
        await ExecuteStoredProcedureAsync<object>(procedureName, parameters);

        // Extract automatically
        return new CommonRes
        {
            Code = parameters.Get<int>("@ResponseCode"),
            Message = parameters.Get<string>("@ResponseMessage")
        };
    }
}