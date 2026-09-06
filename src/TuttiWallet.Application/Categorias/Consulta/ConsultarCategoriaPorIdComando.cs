namespace TuttiWallet.Application.Categorias.Consulta;

public sealed class ConsultarCategoriaPorIdComando
{
    public required Guid UsuarioId { get; init; }
    public required Guid CategoriaId { get; init; }
}
