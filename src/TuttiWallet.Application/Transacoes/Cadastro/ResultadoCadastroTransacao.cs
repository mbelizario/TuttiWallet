namespace TuttiWallet.Application.Transacoes.Cadastro;

public sealed class ResultadoCadastroTransacao
{
    public StatusCadastroTransacao Status { get; }
    public Guid? TransacaoId { get; }
    public IReadOnlyDictionary<string, string[]> Erros { get; }

    private ResultadoCadastroTransacao(
        StatusCadastroTransacao status,
        Guid? transacaoId,
        IReadOnlyDictionary<string, string[]> erros)
    {
        Status = status;
        TransacaoId = transacaoId;
        Erros = erros;
    }

    public static ResultadoCadastroTransacao ComSucesso(Guid transacaoId) =>
        new(StatusCadastroTransacao.Sucesso, transacaoId, new Dictionary<string, string[]>());

    public static ResultadoCadastroTransacao ComDadosInvalidos(Dictionary<string, List<string>> erros) =>
        new(
            StatusCadastroTransacao.DadosInvalidos,
            null,
            erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
}
