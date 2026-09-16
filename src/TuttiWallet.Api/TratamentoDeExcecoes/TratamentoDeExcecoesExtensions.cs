using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace TuttiWallet.Api.TratamentoDeExcecoes;

public static class TratamentoDeExcecoesExtensions
{
    public static IServiceCollection AddTratamentoDeExcecoes(this IServiceCollection services) =>
        // Por padrão, fora do ambiente Development, o Minimal API não lança exceção quando o
        // corpo da requisição não pode ser convertido (ex.: data em formato inválido) — ele
        // mesmo escreve um 400 sem corpo, sem passar pelo UseExceptionHandler abaixo. Forçamos
        // o lançamento em qualquer ambiente para que essa falha também vire uma resposta clara.
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

    public static void UseTratamentoDeExcecoes(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
        {
            var excecao = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            var (statusCode, titulo) = excecao is BadHttpRequestException
                ? (StatusCodes.Status400BadRequest, "Não foi possível interpretar os dados enviados na requisição.")
                : (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado. Tente novamente mais tarde.");

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(excecao, "Erro não tratado ao processar a requisição.");
            }

            await Results.Problem(title: titulo, statusCode: statusCode).ExecuteAsync(context);
        }));
    }
}
