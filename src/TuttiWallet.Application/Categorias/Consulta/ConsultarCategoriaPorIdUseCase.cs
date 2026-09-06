namespace TuttiWallet.Application.Categorias.Consulta;

public sealed class ConsultarCategoriaPorIdUseCase(ICategoriaRepository categoriaRepository)
{
    public async Task<ResultadoConsultaCategoriaPorId> ExecutarAsync(ConsultarCategoriaPorIdComando comando)
    {
        var categoria = await categoriaRepository.ObterPorIdAsync(comando.CategoriaId, comando.UsuarioId);

        if (categoria is null)
            return ResultadoConsultaCategoriaPorId.ComCategoriaNaoEncontrada();

        if (categoria.CategoriaPaiId is not null)
            return ResultadoConsultaCategoriaPorId.ComSucesso(categoria, []);

        var subcategorias = await categoriaRepository.ObterSubcategoriasAsync(comando.UsuarioId, categoria.Id);

        return ResultadoConsultaCategoriaPorId.ComSucesso(categoria, subcategorias);
    }
}
