namespace TuttiWallet.Contracts.Categorias;

public sealed class CriarCategoriaRequest
{
    public required string Nome { get; init; }
    public required int TipoId { get; init; }
    public Guid? CategoriaPaiId { get; init; }
}
