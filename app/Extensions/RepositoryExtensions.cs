using TawakalApi.app.Repositories.ProcedureRepo;
using TawakalApi.app.Repositories.Transaction;

namespace TawakalApi.app.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtensions(this IServiceCollection services)
    {

        // Register data access layers and repositories here
        services.AddScoped<IStoredProcedureRepository, SqlServerStoredProcedureRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        return services;
    }
}