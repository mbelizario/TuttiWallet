namespace TuttiWallet.Domain;

public class Transacao
{
    public Guid Id { get; }
    public Guid UsuarioId { get; }
    public Guid CategoriaId { get; }
    public TipoTransacao Tipo { get; }
    public decimal Valor { get; }
    public DateOnly DataOcorrencia { get; }
    public string? Descricao { get; }

    public Transacao(
        Guid id,
        Guid usuarioId,
        Guid categoriaId,
        TipoTransacao tipo,
        decimal valor,
        DateOnly dataOcorrencia,
        string? descricao = null)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor da transação deve ser maior que zero.");
        }

        Id = id;
        UsuarioId = usuarioId;
        CategoriaId = categoriaId;
        Tipo = tipo;
        Valor = valor;
        DataOcorrencia = dataOcorrencia;
        Descricao = descricao;
    }
}
