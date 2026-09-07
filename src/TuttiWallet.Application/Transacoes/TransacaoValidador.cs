namespace TuttiWallet.Application.Transacoes;

public static class TransacaoValidador
{
    public static void AdicionarErro(Dictionary<string, List<string>> erros, string campo, string mensagem)
    {
        if (!erros.TryGetValue(campo, out var listaDeErros))
        {
            listaDeErros = [];
            erros[campo] = listaDeErros;
        }

        listaDeErros.Add(mensagem);
    }
}
