using Microsoft.Extensions.DependencyInjection;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Cadastro;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Application.Usuarios.Cadastro;

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
