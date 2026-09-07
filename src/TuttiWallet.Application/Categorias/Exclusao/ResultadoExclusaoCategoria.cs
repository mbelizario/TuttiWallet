namespace TuttiWallet.Application.Categorias.Exclusao;

public sealed class ResultadoExclusaoCategoria
{
    public StatusExclusaoCategoria Status { get; }
    public IReadOnlyDictionary<string, string[]> Erros { get; }

    private ResultadoExclusaoCategoria(StatusExclusaoCategoria status, IReadOnlyDictionary<string, string[]> erros)
    {
        Status = status;
        Erros = erros;
    }

    public static ResultadoExclusaoCategoria ComSucesso() =>
        new(StatusExclusaoCategoria.Sucesso, new Dictionary<string, string[]>());

    public static ResultadoExclusaoCategoria ComCategoriaNaoEncontrada() =>
        new(StatusExclusaoCategoria.NaoEncontrada, new Dictionary<string, string[]>());

    public static ResultadoExclusaoCategoria ComDadosInvalidos(Dictionary<string, List<string>> erros) =>
        new(
            StatusExclusaoCategoria.DadosInvalidos,
            erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
}
