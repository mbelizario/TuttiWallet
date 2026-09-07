namespace TuttiWallet.Contracts.Transacoes;

public sealed class CriarTransacaoRequest
{
    public required Guid CategoriaId { get; init; }
    public required decimal Valor { get; init; }
    public required DateOnly DataOcorrencia { get; init; }
    public required string Descricao { get; init; }
    public string? Observacoes { get; init; }
}
