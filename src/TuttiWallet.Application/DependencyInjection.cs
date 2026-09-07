using Microsoft.Extensions.DependencyInjection;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Cadastro;
using TuttiWallet.Application.Categorias.Consulta;
using TuttiWallet.Application.Categorias.Edicao;
using TuttiWallet.Application.Categorias.Exclusao;
using TuttiWallet.Application.Categorias.Listagem;
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
        services.AddScoped<ConsultarCategoriaPorIdUseCase>();
        services.AddScoped<ListarCategoriasUseCase>();
        services.AddScoped<EditarCategoriaUseCase>();
        services.AddScoped<ExcluirCategoriaUseCase>();

        return services;
    }
}
