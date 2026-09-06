namespace TuttiWallet.Application.Categorias.Edicao;

public sealed class ResultadoEdicaoCategoria
{
    public StatusEdicaoCategoria Status { get; }
    public IReadOnlyDictionary<string, string[]> Erros { get; }

    private ResultadoEdicaoCategoria(StatusEdicaoCategoria status, IReadOnlyDictionary<string, string[]> erros)
    {
        Status = status;
        Erros = erros;
    }

    public static ResultadoEdicaoCategoria ComSucesso() =>
        new(StatusEdicaoCategoria.Sucesso, new Dictionary<string, string[]>());

    public static ResultadoEdicaoCategoria ComCategoriaNaoEncontrada() =>
        new(StatusEdicaoCategoria.NaoEncontrada, new Dictionary<string, string[]>());

    public static ResultadoEdicaoCategoria ComDadosInvalidos(Dictionary<string, List<string>> erros) =>
        new(
            StatusEdicaoCategoria.DadosInvalidos,
            erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
}
