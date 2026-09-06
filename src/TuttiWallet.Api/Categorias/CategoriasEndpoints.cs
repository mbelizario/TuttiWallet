using System.Security.Claims;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Contracts.Categorias;

namespace TuttiWallet.Api.Categorias;

public static class CategoriasEndpoints
{
    public static IEndpointRouteBuilder MapCategoriasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categorias", CriarCategoriaAsync).RequireAuthorization();

        return app;
    }

    private static async Task<IResult> CriarCategoriaAsync(
        CriarCategoriaRequest request,
        ClaimsPrincipal usuarioLogado,
        CadastrarCategoriaUseCase useCase)
    {
        var usuarioId = Guid.Parse(usuarioLogado.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var comando = new CadastrarCategoriaComando
        {
            UsuarioId = usuarioId,
            Nome = request.Nome,
            TipoId = request.TipoId,
            CategoriaPaiId = request.CategoriaPaiId
        };

        var resultado = await useCase.ExecutarAsync(comando);

        return resultado.Status switch
        {
            StatusCadastroCategoria.Sucesso => TypedResults.Created(
                $"/api/categorias/{resultado.CategoriaId}",
                new CategoriaResponse { Id = resultado.CategoriaId!.Value }),
            StatusCadastroCategoria.DadosInvalidos => TypedResults.ValidationProblem(resultado.Erros),
            _ => TypedResults.Problem()
        };
    }
}
