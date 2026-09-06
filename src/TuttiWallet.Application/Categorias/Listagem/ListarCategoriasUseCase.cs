namespace TuttiWallet.Application.Categorias.Listagem;

public sealed class ListarCategoriasUseCase(ICategoriaRepository categoriaRepository)
{
    public async Task<ResultadoListagemCategorias> ExecutarAsync(ListarCategoriasComando comando)
    {
        var erros = ListagemCategoriasValidador.Validar(comando);

        if (erros.Count > 0)
            return ResultadoListagemCategorias.ComDadosInvalidos(erros);

        var totalRegistros = await categoriaRepository.ObterQuantidadeAsync(comando.UsuarioId);
        var categorias = await categoriaRepository.ObterPaginadoAsync(comando.UsuarioId, comando.Pagina, comando.TamanhoPagina);

        return ResultadoListagemCategorias.ComSucesso(categorias, totalRegistros);
    }
}
