namespace TuttiWallet.Application.Categorias.Edicao;

public sealed class EditarCategoriaComando
{
    public required Guid UsuarioId { get; init; }
    public required Guid CategoriaId { get; init; }
    public required string Nome { get; init; }
    public required int TipoId { get; init; }
    public Guid? CategoriaPaiId { get; init; }
}
