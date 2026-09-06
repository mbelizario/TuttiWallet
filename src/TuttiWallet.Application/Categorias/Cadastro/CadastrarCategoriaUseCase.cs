using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias.Cadastro;

public sealed class CadastrarCategoriaUseCase(ICategoriaRepository categoriaRepository)
{
    public async Task<ResultadoCadastroCategoria> ExecutarAsync(CadastrarCategoriaComando comando)
    {
        var erros = CadastroCategoriaValidador.Validar(comando);

        Categoria? categoriaPai = null;

        if (comando.CategoriaPaiId is Guid categoriaPaiId)
        {
            categoriaPai = await categoriaRepository.ObterPorIdAsync(categoriaPaiId, comando.UsuarioId);

            if (categoriaPai is null)
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.CategoriaPaiId), "Categoria pai não encontrada.");
            }
            else if (categoriaPai.CategoriaPaiId is not null)
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.CategoriaPaiId), "A categoria pai não pode ser uma subcategoria.");
            }
            else if (!erros.ContainsKey(nameof(comando.TipoId)) && (int)categoriaPai.Tipo != comando.TipoId)
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.TipoId), "O tipo deve ser igual ao da categoria pai.");
            }
        }

        if (!erros.ContainsKey(nameof(comando.Nome)))
        {
            var nomesExistentes = await categoriaRepository.ObterNomesPorPaiAsync(comando.UsuarioId, comando.CategoriaPaiId);
            var nomeNormalizado = CategoriaValidador.NormalizarNome(comando.Nome);

            if (nomesExistentes.Any(nome => CategoriaValidador.NormalizarNome(nome) == nomeNormalizado))
                CategoriaValidador.AdicionarErro(erros, nameof(comando.Nome), "Já existe uma categoria com este nome.");
        }

        if (erros.Count > 0)
            return ResultadoCadastroCategoria.ComDadosInvalidos(erros);

        var categoria = new Categoria(
            Guid.NewGuid(),
            comando.UsuarioId,
            comando.Nome,
            (TipoTransacao)comando.TipoId,
            categoriaPai?.Id);

        await categoriaRepository.InserirAsync(categoria);

        return ResultadoCadastroCategoria.ComSucesso(categoria.Id);
    }
}
