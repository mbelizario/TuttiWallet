namespace TuttiWallet.Contracts.Categorias;

public sealed class CategoriaDetalheResponse
{
    public required Guid Id { get; init; }
    public required string Nome { get; init; }
    public required int TipoId { get; init; }
    public Guid? CategoriaPaiId { get; init; }
    public IReadOnlyList<SubcategoriaResponse>? Subcategorias { get; init; }
}
