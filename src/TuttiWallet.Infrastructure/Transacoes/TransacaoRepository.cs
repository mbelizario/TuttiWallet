using Dapper;
using Npgsql;
using TuttiWallet.Application.Transacoes;
using TuttiWallet.Domain;

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

    public async Task InserirAsync(Transacao transacao)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        await conexao.ExecuteAsync(
            """
            INSERT INTO Transacoes (Id, UsuarioId, CategoriaId, Valor, DataOcorrencia, Descricao, Observacoes)
            VALUES (@Id, @UsuarioId, @CategoriaId, @Valor, @DataOcorrencia, @Descricao, @Observacoes);
            """,
            new
            {
                transacao.Id,
                transacao.UsuarioId,
                transacao.CategoriaId,
                transacao.Valor,
                transacao.DataOcorrencia,
                transacao.Descricao,
                transacao.Observacoes
            });
    }
}
