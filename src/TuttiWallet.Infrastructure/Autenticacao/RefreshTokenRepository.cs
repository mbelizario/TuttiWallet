using Dapper;
using Npgsql;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Domain;

namespace TuttiWallet.Infrastructure.Autenticacao;

public sealed class RefreshTokenRepository(NpgsqlDataSource dataSource) : IRefreshTokenRepository
{
    public async Task InserirAsync(RefreshToken refreshToken)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();
        await conexao.ExecuteAsync(
            """
            INSERT INTO RefreshTokens (Id, UsuarioId, HashToken, CriadoEm, ExpiraEm)
            VALUES (@Id, @UsuarioId, @HashToken, @CriadoEm, @ExpiraEm);
            """,
            new
            {
                refreshToken.Id,
                refreshToken.UsuarioId,
                refreshToken.HashToken,
                refreshToken.CriadoEm,
                refreshToken.ExpiraEm
            });
    }
}
