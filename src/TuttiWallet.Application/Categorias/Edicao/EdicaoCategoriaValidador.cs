using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias.Edicao;

public static class EdicaoCategoriaValidador
{
    public static Dictionary<string, List<string>> Validar(EditarCategoriaComando comando)
    {
        var erros = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(comando.Nome))
            CategoriaValidador.AdicionarErro(erros, nameof(comando.Nome), "Este campo é obrigatório.");

        if (!Enum.IsDefined(typeof(TipoTransacao), comando.TipoId))
            CategoriaValidador.AdicionarErro(erros, nameof(comando.TipoId), "Tipo inválido.");

        return erros;
    }
}
