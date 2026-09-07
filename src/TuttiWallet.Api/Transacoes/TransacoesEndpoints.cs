using System.Security.Claims;
using TuttiWallet.Application.Transacoes.Cadastro;
using TuttiWallet.Contracts.Transacoes;

namespace TuttiWallet.Api.Transacoes;

public static class TransacoesEndpoints
{
    public static IEndpointRouteBuilder MapTransacoesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/transacoes", CriarTransacaoAsync).RequireAuthorization();

        return app;
    }

    private static async Task<IResult> CriarTransacaoAsync(
        CriarTransacaoRequest request,
        ClaimsPrincipal usuarioLogado,
        CadastrarTransacaoUseCase useCase)
    {
        var usuarioId = Guid.Parse(usuarioLogado.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var comando = new CadastrarTransacaoComando
        {
            UsuarioId = usuarioId,
            CategoriaId = request.CategoriaId,
            Valor = request.Valor,
            DataOcorrencia = request.DataOcorrencia,
            Descricao = request.Descricao,
            Observacoes = request.Observacoes
        };

        var resultado = await useCase.ExecutarAsync(comando);

        return resultado.Status switch
        {
            StatusCadastroTransacao.Sucesso => TypedResults.Created(
                $"/api/transacoes/{resultado.TransacaoId}",
                new TransacaoResponse { Id = resultado.TransacaoId!.Value }),
            StatusCadastroTransacao.DadosInvalidos => TypedResults.ValidationProblem(resultado.Erros),
            _ => TypedResults.Problem()
        };
    }
}
