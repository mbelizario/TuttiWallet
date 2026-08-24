using System.Text.RegularExpressions;

namespace TuttiWallet.Application.Usuarios;

public static partial class CadastroUsuarioValidador
{
    private const int TamanhoMinimoNome = 2;
    private const int TamanhoMaximoNome = 100;
    private const int TamanhoMinimoSenha = 6;
    private const int TamanhoMaximoSenha = 18;

    public static Dictionary<string, List<string>> Validar(CadastrarPrimeiroUsuarioComando comando)
    {
        var erros = new Dictionary<string, List<string>>();

        ValidarNome(comando.Nome, nameof(comando.Nome), erros);
        ValidarNome(comando.Sobrenome, nameof(comando.Sobrenome), erros);
        ValidarEmail(comando.Email, erros);
        ValidarCelular(comando.Celular, erros);
        ValidarSenha(comando.Senha, comando.ConfirmacaoSenha, erros);

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

    private static void ValidarNome(string valor, string campo, Dictionary<string, List<string>> erros)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            AdicionarErro(erros, campo, "Este campo é obrigatório.");
            return;
        }

        if (valor.Length is < TamanhoMinimoNome or > TamanhoMaximoNome)
        {
            AdicionarErro(erros, campo, $"Deve ter entre {TamanhoMinimoNome} e {TamanhoMaximoNome} caracteres.");
        }

        if (!RegexNome().IsMatch(valor))
        {
            AdicionarErro(erros, campo, "Deve conter apenas letras e espaços.");
        }
    }

    private static void ValidarEmail(string valor, Dictionary<string, List<string>> erros)
    {
        const string campo = nameof(CadastrarPrimeiroUsuarioComando.Email);

        if (string.IsNullOrWhiteSpace(valor))
        {
            AdicionarErro(erros, campo, "Este campo é obrigatório.");
            return;
        }

        if (!RegexEmail().IsMatch(valor))
        {
            AdicionarErro(erros, campo, "Formato de e-mail inválido.");
        }
    }

    private static void ValidarCelular(string valor, Dictionary<string, List<string>> erros)
    {
        const string campo = nameof(CadastrarPrimeiroUsuarioComando.Celular);

        if (string.IsNullOrWhiteSpace(valor))
        {
            AdicionarErro(erros, campo, "Este campo é obrigatório.");
            return;
        }

        if (!RegexCelular().IsMatch(valor))
        {
            AdicionarErro(erros, campo, "Formato inválido. Use (99) 99999-9999.");
        }
    }

    private static void ValidarSenha(string senha, string confirmacaoSenha, Dictionary<string, List<string>> erros)
    {
        const string campoSenha = nameof(CadastrarPrimeiroUsuarioComando.Senha);

        if (string.IsNullOrWhiteSpace(senha))
        {
            AdicionarErro(erros, campoSenha, "Este campo é obrigatório.");
            return;
        }

        if (senha.Length is < TamanhoMinimoSenha or > TamanhoMaximoSenha)
        {
            AdicionarErro(erros, campoSenha, $"Deve ter entre {TamanhoMinimoSenha} e {TamanhoMaximoSenha} caracteres.");
        }

        if (!senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
        {
            AdicionarErro(erros, campoSenha, "Deve conter letras e números.");
        }

        if (ContemSequenciaNumericaObvia(senha))
        {
            AdicionarErro(erros, campoSenha, "Não pode conter sequências numéricas óbvias, como 123 ou 456.");
        }

        if (senha != confirmacaoSenha)
        {
            AdicionarErro(
                erros,
                nameof(CadastrarPrimeiroUsuarioComando.ConfirmacaoSenha),
                "A confirmação de senha não confere com a senha.");
        }
    }

    private static bool ContemSequenciaNumericaObvia(string senha)
    {
        for (var i = 0; i < senha.Length - 2; i++)
        {
            if (!char.IsDigit(senha[i]) || !char.IsDigit(senha[i + 1]) || !char.IsDigit(senha[i + 2]))
            {
                continue;
            }

            var primeiro = senha[i] - '0';
            var segundo = senha[i + 1] - '0';
            var terceiro = senha[i + 2] - '0';

            var crescente = segundo == primeiro + 1 && terceiro == segundo + 1;
            var decrescente = segundo == primeiro - 1 && terceiro == segundo - 1;

            if (crescente || decrescente)
            {
                return true;
            }
        }

        return false;
    }

    [GeneratedRegex(@"^[A-Za-zÀ-ÖØ-öø-ÿ ]+$")]
    private static partial Regex RegexNome();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex RegexEmail();

    [GeneratedRegex(@"^\(\d{2}\) \d{5}-\d{4}$")]
    private static partial Regex RegexCelular();
}
