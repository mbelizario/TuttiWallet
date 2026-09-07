namespace TuttiWallet.Domain;

public class Transacao
{
    public Guid Id { get; }
    public Guid UsuarioId { get; }
    public Guid CategoriaId { get; }
    public decimal Valor { get; }
    public DateOnly DataOcorrencia { get; }
    public string Descricao { get; }
    public string? Observacoes { get; }

    public Transacao(
        Guid id,
        Guid usuarioId,
        Guid categoriaId,
        decimal valor,
        DateOnly dataOcorrencia,
        string descricao,
        string? observacoes = null)
    {
        if (valor <= 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor da transação deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("A descrição da transação é obrigatória.", nameof(descricao));

        if (descricao.Length > 100)
            throw new ArgumentException("A descrição da transação deve ter no máximo 100 caracteres.", nameof(descricao));

        Id = id;
        UsuarioId = usuarioId;
        CategoriaId = categoriaId;
        Valor = valor;
        DataOcorrencia = dataOcorrencia;
        Descricao = descricao;
        Observacoes = observacoes;
    }
}
