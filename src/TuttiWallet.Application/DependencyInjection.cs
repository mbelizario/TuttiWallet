using Microsoft.Extensions.DependencyInjection;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Usuarios;

namespace TuttiWallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CadastrarPrimeiroUsuarioUseCase>();
        services.AddScoped<AutenticarUsuarioUseCase>();
        services.AddScoped<CadastrarCategoriaUseCase>();

        return services;
    }
}
