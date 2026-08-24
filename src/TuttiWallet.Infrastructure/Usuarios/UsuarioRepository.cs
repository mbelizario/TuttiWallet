using Dapper;
using Npgsql;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Domain;

namespace TuttiWallet.Infrastructure.Usuarios;

public sealed class UsuarioRepository(NpgsqlDataSource dataSource) : IUsuarioRepository
{
    public async Task<int> ObterQuantidadeUsuariosAsync()
    {
        await using var conexao = await dataSource.OpenConnectionAsync();
        return await conexao.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Usuarios;");
    }

    public async Task<bool> ObterExistePorEmailAsync(string email)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();
        return await conexao.ExecuteScalarAsync<bool>(
            "SELECT EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email);",
            new { Email = email });
    }

    public async Task InserirAsync(Usuario usuario)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();
        await conexao.ExecuteAsync(
            """
            INSERT INTO Usuarios (Id, Nome, Sobrenome, Email, Celular, HashSenha)
            VALUES (@Id, @Nome, @Sobrenome, @Email, @Celular, @HashSenha);
            """,
            new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Sobrenome,
                usuario.Email,
                usuario.Celular,
                usuario.HashSenha
            });
    }
}
