namespace TuttiWallet.Application.Transacoes;

public interface ITransacaoRepository
{
    Task<bool> ExisteTransacaoParaCategoriaAsync(Guid categoriaId);
}
