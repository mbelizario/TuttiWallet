namespace TuttiWallet.Application.Transacoes.Cadastro;

public static class CadastroTransacaoValidador
{
    public static Dictionary<string, List<string>> Validar(CadastrarTransacaoComando comando)
    {
        var erros = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(comando.Descricao))
            TransacaoValidador.AdicionarErro(erros, nameof(comando.Descricao), "Este campo é obrigatório.");
        else if (comando.Descricao.Length > 100)
            TransacaoValidador.AdicionarErro(erros, nameof(comando.Descricao), "Este campo deve ter no máximo 100 caracteres.");

        if (comando.Valor <= 0)
            TransacaoValidador.AdicionarErro(erros, nameof(comando.Valor), "O valor deve ser maior que zero.");

        return erros;
    }
}
