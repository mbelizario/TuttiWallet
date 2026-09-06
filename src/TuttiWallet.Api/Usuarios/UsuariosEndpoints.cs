using TuttiWallet.Application.Usuarios;
using TuttiWallet.Application.Usuarios.Cadastro;
using TuttiWallet.Contracts.Usuarios;

namespace TuttiWallet.Api.Usuarios;

public static class UsuariosEndpoints
{
    public static IEndpointRouteBuilder MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/usuarios/primeiro-acesso", CadastrarPrimeiroUsuarioAsync);

        return app;
    }

    private static async Task<IResult> CadastrarPrimeiroUsuarioAsync(
        CadastrarPrimeiroUsuarioRequest request,
        CadastrarPrimeiroUsuarioUseCase useCase)
    {
        var comando = new CadastrarPrimeiroUsuarioComando
        {
            Nome = request.Nome,
            Sobrenome = request.Sobrenome,
            Email = request.Email,
            Celular = request.Celular,
            Senha = request.Senha,
            ConfirmacaoSenha = request.ConfirmacaoSenha
        };

        var resultado = await useCase.ExecutarAsync(comando);

        return resultado.Status switch
        {
            StatusCadastroPrimeiroUsuario.Sucesso => TypedResults.Created(
                $"/api/usuarios/{resultado.UsuarioId}",
                new UsuarioResponse { Id = resultado.UsuarioId!.Value }),
            StatusCadastroPrimeiroUsuario.UsuarioJaExiste => TypedResults.Conflict(
                new { mensagem = "Já existe um usuário cadastrado. O cadastro de novos usuários deve ser feito na área logada." }),
            StatusCadastroPrimeiroUsuario.DadosInvalidos => TypedResults.ValidationProblem(resultado.Erros),
            _ => TypedResults.Problem()
        };
    }
}
