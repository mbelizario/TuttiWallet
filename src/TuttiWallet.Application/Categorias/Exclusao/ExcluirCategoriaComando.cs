namespace TuttiWallet.Application.Categorias.Exclusao;

public sealed class ExcluirCategoriaComando
{
    public required Guid UsuarioId { get; init; }
    public required Guid CategoriaId { get; init; }
}
