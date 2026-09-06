using Dapper;
using Npgsql;
using TuttiWallet.Application.Transacoes;

namespace TuttiWallet.Infrastructure.Transacoes;

public sealed class TransacaoRepository(NpgsqlDataSource dataSource) : ITransacaoRepository
{
    public async Task<bool> ExisteTransacaoParaCategoriaAsync(Guid categoriaId)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        return await conexao.QuerySingleAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM Transacoes WHERE CategoriaId = @CategoriaId);",
            new { CategoriaId = categoriaId });
    }
}
