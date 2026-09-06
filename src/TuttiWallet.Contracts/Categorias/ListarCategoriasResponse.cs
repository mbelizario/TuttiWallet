namespace TuttiWallet.Contracts.Categorias;

public sealed class ListarCategoriasResponse
{
    public required int TotalRegistros { get; init; }
    public required int TotalPaginas { get; init; }
    public required int Pagina { get; init; }
    public required int TamanhoPagina { get; init; }
    public required IReadOnlyList<CategoriaListaResponse> Itens { get; init; }
}
