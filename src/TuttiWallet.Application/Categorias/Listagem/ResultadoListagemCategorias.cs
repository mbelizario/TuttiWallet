using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias.Listagem;

public sealed class ResultadoListagemCategorias
{
    public StatusListagemCategorias Status { get; }
    public IReadOnlyList<Categoria> Categorias { get; }
    public int TotalRegistros { get; }
    public IReadOnlyDictionary<string, string[]> Erros { get; }

    private ResultadoListagemCategorias(
        StatusListagemCategorias status,
        IReadOnlyList<Categoria> categorias,
        int totalRegistros,
        IReadOnlyDictionary<string, string[]> erros)
    {
        Status = status;
        Categorias = categorias;
        TotalRegistros = totalRegistros;
        Erros = erros;
    }

    public static ResultadoListagemCategorias ComSucesso(IReadOnlyList<Categoria> categorias, int totalRegistros) =>
        new(StatusListagemCategorias.Sucesso, categorias, totalRegistros, new Dictionary<string, string[]>());

    public static ResultadoListagemCategorias ComDadosInvalidos(Dictionary<string, List<string>> erros) =>
        new(StatusListagemCategorias.DadosInvalidos, [], 0, erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
}
