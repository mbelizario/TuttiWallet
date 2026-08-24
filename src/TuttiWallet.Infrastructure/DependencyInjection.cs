using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Infrastructure.HealthChecks;
using TuttiWallet.Infrastructure.Usuarios;

namespace TuttiWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

        services.AddHealthChecks()
            .AddCheck<PostgresHealthCheck>("postgres");

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<ISenhaHasher, SenhaHasher>();

        return services;
    }
}
