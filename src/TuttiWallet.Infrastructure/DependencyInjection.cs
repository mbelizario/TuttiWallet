using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Infrastructure.Autenticacao;
using TuttiWallet.Infrastructure.HealthChecks;
using TuttiWallet.Infrastructure.Usuarios;

namespace TuttiWallet.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString));

        services.AddHealthChecks()
            .AddCheck<PostgresHealthCheck>("postgres");

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddSingleton<ISenhaHasher, SenhaHasher>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IGeradorToken, GeradorToken>();

        return services;
    }
}
