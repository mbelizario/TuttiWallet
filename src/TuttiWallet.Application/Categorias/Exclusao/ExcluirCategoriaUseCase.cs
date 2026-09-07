using TuttiWallet.Application.Transacoes;

namespace TuttiWallet.Application.Categorias.Exclusao;

public sealed class ExcluirCategoriaUseCase(ICategoriaRepository categoriaRepository, ITransacaoRepository transacaoRepository)
{
    public async Task<ResultadoExclusaoCategoria> ExecutarAsync(ExcluirCategoriaComando comando)
    {
        var categoria = await categoriaRepository.ObterPorIdAsync(comando.CategoriaId, comando.UsuarioId);

        if (categoria is null)
            return ResultadoExclusaoCategoria.ComCategoriaNaoEncontrada();

        var erros = new Dictionary<string, List<string>>();

        if (categoria.CategoriaPaiId is null)
        {
            var possuiSubcategorias = (await categoriaRepository.ObterSubcategoriasAsync(comando.UsuarioId, categoria.Id)).Count > 0;

            if (possuiSubcategorias)
            {
                CategoriaValidador.AdicionarErro(
                    erros, nameof(comando.CategoriaId), "Não é possível excluir uma categoria que possui subcategorias.");
            }
        }

        if (await transacaoRepository.ExisteTransacaoParaCategoriaAsync(comando.CategoriaId))
        {
            CategoriaValidador.AdicionarErro(
                erros, nameof(comando.CategoriaId), "Não é possível excluir uma categoria que possui transações vinculadas.");
        }

        if (erros.Count > 0)
            return ResultadoExclusaoCategoria.ComDadosInvalidos(erros);

        await categoriaRepository.ExcluirAsync(categoria.Id, comando.UsuarioId);

        return ResultadoExclusaoCategoria.ComSucesso();
    }
}
