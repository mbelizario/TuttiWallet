namespace TuttiWallet.Application.Categorias.Cadastro;

public sealed class ResultadoCadastroCategoria
{
    public StatusCadastroCategoria Status { get; }
    public Guid? CategoriaId { get; }
    public IReadOnlyDictionary<string, string[]> Erros { get; }

    private ResultadoCadastroCategoria(
        StatusCadastroCategoria status,
        Guid? categoriaId,
        IReadOnlyDictionary<string, string[]> erros)
    {
        Status = status;
        CategoriaId = categoriaId;
        Erros = erros;
    }

    public static ResultadoCadastroCategoria ComSucesso(Guid categoriaId) =>
        new(StatusCadastroCategoria.Sucesso, categoriaId, new Dictionary<string, string[]>());

    public static ResultadoCadastroCategoria ComDadosInvalidos(Dictionary<string, List<string>> erros) =>
        new(
            StatusCadastroCategoria.DadosInvalidos,
            null,
            erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
}
