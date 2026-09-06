using Dapper;
using Npgsql;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Domain;

namespace TuttiWallet.Infrastructure.Categorias;

public sealed class CategoriaRepository(NpgsqlDataSource dataSource) : ICategoriaRepository
{
    public async Task<Categoria?> ObterPorIdAsync(Guid id, Guid usuarioId)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        var registro = await conexao.QuerySingleOrDefaultAsync<CategoriaRegistro>(
            """
            SELECT Id, UsuarioId, Nome, TipoId, CategoriaPaiId
            FROM Categorias
            WHERE Id = @Id AND UsuarioId = @UsuarioId;
            """,
            new { Id = id, UsuarioId = usuarioId });

        return registro is null
            ? null
            : new Categoria(registro.Id, registro.UsuarioId, registro.Nome, (TipoTransacao)registro.TipoId, registro.CategoriaPaiId);
    }

    public async Task<IReadOnlyList<Categoria>> ObterSubcategoriasAsync(Guid usuarioId, Guid categoriaPaiId)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        var registros = await conexao.QueryAsync<CategoriaRegistro>(
            """
            SELECT Id, UsuarioId, Nome, TipoId, CategoriaPaiId
            FROM Categorias
            WHERE UsuarioId = @UsuarioId AND CategoriaPaiId = @CategoriaPaiId;
            """,
            new { UsuarioId = usuarioId, CategoriaPaiId = categoriaPaiId });

        return registros
            .Select(registro => new Categoria(registro.Id, registro.UsuarioId, registro.Nome, (TipoTransacao)registro.TipoId, registro.CategoriaPaiId))
            .ToList();
    }

    public async Task<IReadOnlyList<string>> ObterNomesPorPaiAsync(Guid usuarioId, Guid? categoriaPaiId)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        var nomes = await conexao.QueryAsync<string>(
            """
            SELECT Nome
            FROM Categorias
            WHERE UsuarioId = @UsuarioId AND CategoriaPaiId IS NOT DISTINCT FROM @CategoriaPaiId;
            """,
            new { UsuarioId = usuarioId, CategoriaPaiId = categoriaPaiId });

        return nomes.ToList();
    }

    public async Task InserirAsync(Categoria categoria)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        await conexao.ExecuteAsync(
            """
            INSERT INTO Categorias (Id, UsuarioId, Nome, TipoId, CategoriaPaiId)
            VALUES (@Id, @UsuarioId, @Nome, @TipoId, @CategoriaPaiId);
            """,
            new
            {
                categoria.Id,
                categoria.UsuarioId,
                categoria.Nome,
                TipoId = (int)categoria.Tipo,
                categoria.CategoriaPaiId
            });
    }

    public async Task AtualizarAsync(Categoria categoria)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        await conexao.ExecuteAsync(
            """
            UPDATE Categorias
            SET Nome = @Nome, TipoId = @TipoId, CategoriaPaiId = @CategoriaPaiId
            WHERE Id = @Id AND UsuarioId = @UsuarioId;
            """,
            new
            {
                categoria.Id,
                categoria.UsuarioId,
                categoria.Nome,
                TipoId = (int)categoria.Tipo,
                categoria.CategoriaPaiId
            });
    }

    public async Task<int> ObterQuantidadeAsync(Guid usuarioId)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        return await conexao.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM Categorias WHERE UsuarioId = @UsuarioId;",
            new { UsuarioId = usuarioId });
    }

    public async Task<IReadOnlyList<Categoria>> ObterPaginadoAsync(Guid usuarioId, int pagina, int tamanhoPagina)
    {
        await using var conexao = await dataSource.OpenConnectionAsync();

        var registros = await conexao.QueryAsync<CategoriaRegistro>(
            """
            SELECT Id, UsuarioId, Nome, TipoId, CategoriaPaiId
            FROM Categorias
            WHERE UsuarioId = @UsuarioId
            ORDER BY Nome
            OFFSET @Deslocamento LIMIT @TamanhoPagina;
            """,
            new { UsuarioId = usuarioId, Deslocamento = (pagina - 1) * tamanhoPagina, TamanhoPagina = tamanhoPagina });

        return registros
            .Select(registro => new Categoria(registro.Id, registro.UsuarioId, registro.Nome, (TipoTransacao)registro.TipoId, registro.CategoriaPaiId))
            .ToList();
    }

    private sealed class CategoriaRegistro
    {
        public Guid Id { get; init; }
        public Guid UsuarioId { get; init; }
        public string Nome { get; init; } = string.Empty;
        public int TipoId { get; init; }
        public Guid? CategoriaPaiId { get; init; }
    }
}
