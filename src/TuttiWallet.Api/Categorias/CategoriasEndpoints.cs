using System.Security.Claims;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Cadastro;
using TuttiWallet.Application.Categorias.Consulta;
using TuttiWallet.Contracts.Categorias;
using TuttiWallet.Domain;

namespace TuttiWallet.Api.Categorias;

public static class CategoriasEndpoints
{
    public static IEndpointRouteBuilder MapCategoriasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categorias", CriarCategoriaAsync).RequireAuthorization();
        app.MapGet("/api/categorias/{id:guid}", ConsultarCategoriaPorIdAsync).RequireAuthorization();

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

    private static async Task<IResult> ConsultarCategoriaPorIdAsync(
        Guid id,
        ClaimsPrincipal usuarioLogado,
        ConsultarCategoriaPorIdUseCase useCase)
    {
        var usuarioId = Guid.Parse(usuarioLogado.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var comando = new ConsultarCategoriaPorIdComando
        {
            UsuarioId = usuarioId,
            CategoriaId = id
        };

        var resultado = await useCase.ExecutarAsync(comando);

        return resultado.Status switch
        {
            StatusConsultaCategoriaPorId.Sucesso => TypedResults.Ok(MapearParaResponse(resultado.Categoria!, resultado.Subcategorias)),
            StatusConsultaCategoriaPorId.NaoEncontrada => TypedResults.NotFound(),
            _ => TypedResults.Problem()
        };
    }

    private static CategoriaDetalheResponse MapearParaResponse(Categoria categoria, IReadOnlyList<Categoria> subcategorias) =>
        new()
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            TipoId = (int)categoria.Tipo,
            CategoriaPaiId = categoria.CategoriaPaiId,
            Subcategorias = categoria.CategoriaPaiId is null
                ? subcategorias.Select(subcategoria => new SubcategoriaResponse { Id = subcategoria.Id, Nome = subcategoria.Nome }).ToList()
                : null
        };
}
