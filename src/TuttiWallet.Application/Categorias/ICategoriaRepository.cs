using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias;

public interface ICategoriaRepository
{
    Task<Categoria?> ObterPorIdAsync(Guid id, Guid usuarioId);
    Task<IReadOnlyList<Categoria>> ObterSubcategoriasAsync(Guid usuarioId, Guid categoriaPaiId);
    Task<IReadOnlyList<string>> ObterNomesPorPaiAsync(Guid usuarioId, Guid? categoriaPaiId);
    Task InserirAsync(Categoria categoria);
}
