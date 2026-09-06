namespace TuttiWallet.Application.Categorias.Listagem;

public sealed class ListarCategoriasComando
{
    public required Guid UsuarioId { get; init; }
    public required int Pagina { get; init; }
    public required int TamanhoPagina { get; init; }
}
