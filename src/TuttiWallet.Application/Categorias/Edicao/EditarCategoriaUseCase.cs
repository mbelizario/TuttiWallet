using TuttiWallet.Application.Transacoes;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias.Edicao;

public sealed class EditarCategoriaUseCase(ICategoriaRepository categoriaRepository, ITransacaoRepository transacaoRepository)
{
    public async Task<ResultadoEdicaoCategoria> ExecutarAsync(EditarCategoriaComando comando)
    {
        var categoriaAtual = await categoriaRepository.ObterPorIdAsync(comando.CategoriaId, comando.UsuarioId);

        if (categoriaAtual is null)
            return ResultadoEdicaoCategoria.ComCategoriaNaoEncontrada();

        var erros = EdicaoCategoriaValidador.Validar(comando);

        var categoriaPaiAlterada = comando.CategoriaPaiId != categoriaAtual.CategoriaPaiId;

        Categoria? novaCategoriaPai = null;

        if (comando.CategoriaPaiId is Guid categoriaPaiId)
        {
            if (categoriaPaiId == comando.CategoriaId)
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.CategoriaPaiId), "Uma categoria não pode ser subcategoria de si mesma.");
            }
            else
            {
                novaCategoriaPai = await categoriaRepository.ObterPorIdAsync(categoriaPaiId, comando.UsuarioId);

                if (novaCategoriaPai is null)
                {
                    CategoriaValidador.AdicionarErro(
                        erros, nameof(comando.CategoriaPaiId), "Categoria pai não encontrada.");
                }
                else if (novaCategoriaPai.CategoriaPaiId is not null)
                {
                    CategoriaValidador.AdicionarErro(
                        erros, nameof(comando.CategoriaPaiId), "A categoria pai não pode ser uma subcategoria.");
                }
                else if (!erros.ContainsKey(nameof(comando.TipoId)) && (int)novaCategoriaPai.Tipo != comando.TipoId)
                {
                    CategoriaValidador.AdicionarErro(
                        erros, nameof(comando.TipoId), "O tipo deve ser igual ao da categoria pai.");
                }
            }
        }

        if (categoriaPaiAlterada && !erros.ContainsKey(nameof(comando.CategoriaPaiId)))
        {
            var possuiSubcategorias = (await categoriaRepository.ObterSubcategoriasAsync(comando.UsuarioId, categoriaAtual.Id)).Count > 0;

            if (possuiSubcategorias)
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.CategoriaPaiId), "Uma categoria com subcategorias não pode se tornar uma subcategoria.");
            }
            else if (await transacaoRepository.ExisteTransacaoParaCategoriaAsync(comando.CategoriaId))
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.CategoriaPaiId), "Não é possível alterar a categoria pai de uma categoria com transações.");
            }
        }

        if (!erros.ContainsKey(nameof(comando.Nome)))
        {
            var nomesExistentes = (await categoriaRepository.ObterNomesPorPaiAsync(comando.UsuarioId, comando.CategoriaPaiId)).ToList();

            if (!categoriaPaiAlterada)
                nomesExistentes.Remove(categoriaAtual.Nome);

            var nomeNormalizado = CategoriaValidador.NormalizarNome(comando.Nome);

            if (nomesExistentes.Any(nome => CategoriaValidador.NormalizarNome(nome) == nomeNormalizado))
                CategoriaValidador.AdicionarErro(erros, nameof(comando.Nome), "Já existe uma categoria com este nome.");
        }

        if (erros.Count > 0)
            return ResultadoEdicaoCategoria.ComDadosInvalidos(erros);

        var categoriaAtualizada = new Categoria(
            categoriaAtual.Id,
            comando.UsuarioId,
            comando.Nome,
            (TipoTransacao)comando.TipoId,
            novaCategoriaPai?.Id);

        await categoriaRepository.AtualizarAsync(categoriaAtualizada);

        return ResultadoEdicaoCategoria.ComSucesso();
    }
}
