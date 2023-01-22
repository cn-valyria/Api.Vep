using api.Auth;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;

[assembly: FunctionsStartup(typeof(api.Startup))]

namespace api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services
            .AddLogging()
            .AddVepDbRepositories(builder.GetContext().Configuration)
            .AddInternalProviders()
            .AddAutoMapper(typeof(Startup));
    }
}

internal static class StartupExtensions
{
    public static IServiceCollection AddVepDbRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var vepDbConnectionString = configuration.GetConnectionString("VepDb");

        return services
            .AddTransient<IAuthorizationRepository>(sp => new AuthorizationRepository(vepDbConnectionString))
            .AddTransient<IAccountsRepository>(sp => new AccountsRepository(vepDbConnectionString))
            .AddTransient<ITransactionsRepository>(sp => new TransactionsRepository(vepDbConnectionString))
            .AddTransient<IListsRepository>(sp => new ListsRepository(vepDbConnectionString));
    }

    public static IServiceCollection AddInternalProviders(this IServiceCollection services) => services
        .AddTransient<ITokenProvider, TokenProvider>()
        .AddTransient<IAuthenticationProvider, AuthenticationProvider>();
}