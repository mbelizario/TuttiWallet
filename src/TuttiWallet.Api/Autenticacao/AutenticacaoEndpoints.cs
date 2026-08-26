using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Contracts.Autenticacao;

namespace TuttiWallet.Api.Autenticacao;

public static class AutenticacaoEndpoints
{
    public static IEndpointRouteBuilder MapAutenticacaoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/autenticacao/login", AutenticarAsync);

        return app;
    }

    private static async Task<IResult> AutenticarAsync(LoginRequest request, AutenticarUsuarioUseCase useCase)
    {
        var comando = new AutenticarUsuarioComando
        {
            Email = request.Email,
            Senha = request.Senha
        };

        var resultado = await useCase.ExecutarAsync(comando);

        return resultado.Status switch
        {
            StatusAutenticacao.Sucesso => TypedResults.Ok(new LoginResponse
            {
                AccessToken = resultado.AccessToken!,
                RefreshToken = resultado.RefreshToken!,
                ExpiraEm = resultado.ExpiraEm!.Value
            }),
            StatusAutenticacao.CredenciaisInvalidas => TypedResults.Unauthorized(),
            _ => TypedResults.Problem()
        };
    }
}
