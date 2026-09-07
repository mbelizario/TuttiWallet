using TuttiWallet.Domain;

namespace TuttiWallet.Application.Transacoes;

public interface ITransacaoRepository
{
    Task<bool> ExisteTransacaoParaCategoriaAsync(Guid categoriaId);
    Task InserirAsync(Transacao transacao);
}
