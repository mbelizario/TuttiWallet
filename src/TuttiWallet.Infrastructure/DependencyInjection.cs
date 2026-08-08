using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TuttiWallet.Infrastructure.HealthChecks;

namespace TuttiWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

        services.AddHealthChecks()
            .AddCheck<PostgresHealthCheck>("postgres");

        return services;
    }
}
