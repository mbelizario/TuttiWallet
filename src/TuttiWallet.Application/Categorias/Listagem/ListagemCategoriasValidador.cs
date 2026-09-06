namespace TuttiWallet.Application.Categorias.Listagem;

public static class ListagemCategoriasValidador
{
    public const int TamanhoPaginaMaximo = 100;

    public static Dictionary<string, List<string>> Validar(ListarCategoriasComando comando)
    {
        var erros = new Dictionary<string, List<string>>();

        if (comando.Pagina < 1)
            AdicionarErro(erros, nameof(comando.Pagina), "A página deve ser maior ou igual a 1.");

        if (comando.TamanhoPagina < 1 || comando.TamanhoPagina > TamanhoPaginaMaximo)
            AdicionarErro(erros, nameof(comando.TamanhoPagina), $"O tamanho da página deve estar entre 1 e {TamanhoPaginaMaximo}.");

        return erros;
    }

    private static void AdicionarErro(Dictionary<string, List<string>> erros, string campo, string mensagem)
    {
        if (!erros.TryGetValue(campo, out var listaDeErros))
        {
            listaDeErros = [];
            erros[campo] = listaDeErros;
        }

        listaDeErros.Add(mensagem);
    }
}
