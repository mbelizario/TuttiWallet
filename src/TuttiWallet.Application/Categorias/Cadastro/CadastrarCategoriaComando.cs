namespace TuttiWallet.Application.Categorias.Cadastro;

public sealed class CadastrarCategoriaComando
{
    public required Guid UsuarioId { get; init; }
    public required string Nome { get; init; }
    public required int TipoId { get; init; }
    public Guid? CategoriaPaiId { get; init; }
}
