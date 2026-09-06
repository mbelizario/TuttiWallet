using System.Globalization;
using System.Text;

namespace TuttiWallet.Application.Categorias;

public static class CategoriaValidador
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
