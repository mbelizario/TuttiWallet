using Microsoft.Extensions.DependencyInjection;
using TuttiWallet.Application.Usuarios;

namespace TuttiWallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CadastrarPrimeiroUsuarioUseCase>();

        return services;
    }
}
