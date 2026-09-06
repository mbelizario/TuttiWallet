using System.Globalization;
using System.Text;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias.Cadastro;

public static class CadastroCategoriaValidador
{
    public static Dictionary<string, List<string>> Validar(CadastrarCategoriaComando comando)
    {
        var erros = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(comando.Nome))
            AdicionarErro(erros, nameof(comando.Nome), "Este campo é obrigatório.");

        if (!Enum.IsDefined(typeof(TipoTransacao), comando.TipoId))
            AdicionarErro(erros, nameof(comando.TipoId), "Tipo inválido.");

        return erros;
    }

    public static void AdicionarErro(Dictionary<string, List<string>> erros, string campo, string mensagem)
    {
        if (!erros.TryGetValue(campo, out var listaDeErros))
        {
            listaDeErros = [];
            erros[campo] = listaDeErros;
        }

        listaDeErros.Add(mensagem);
    }

    public static string NormalizarNome(string nome)
    {
        var textoSemAcentos = nome
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);

        var textoSemCaracteresEspeciais = new string(textoSemAcentos
            .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            .ToArray());

        var palavras = textoSemCaracteresEspeciais.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return string.Join(' ', palavras).ToUpperInvariant();
    }
}
