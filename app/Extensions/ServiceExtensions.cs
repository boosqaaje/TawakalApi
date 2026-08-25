using TawakalApi.app.Services.Partner;
using TawakalApi.app.Services.PortalAuth;
using TawakalApi.app.Services.Token;
using TawakalApi.app.Services.Transaction;

namespace TawakalApi.app.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection services)
    {
        // Busness logic services registration
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IPartnerService, PartnerService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPortalAuthService, PortalAuthService>();

        return services;
    }
}