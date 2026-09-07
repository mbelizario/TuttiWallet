namespace TuttiWallet.Application.Transacoes.Cadastro;

public sealed class CadastrarTransacaoComando
{
    public required Guid UsuarioId { get; init; }
    public required Guid CategoriaId { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly DataOcorrencia { get; init; }
    public required string Descricao { get; init; }
    public string? Observacoes { get; init; }
}
