using TuttiWallet.Application.Categorias;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Transacoes.Cadastro;

public sealed class CadastrarTransacaoUseCase(ITransacaoRepository transacaoRepository, ICategoriaRepository categoriaRepository)
{
    public async Task<ResultadoCadastroTransacao> ExecutarAsync(CadastrarTransacaoComando comando)
    {
        var erros = CadastroTransacaoValidador.Validar(comando);

        var categoria = await categoriaRepository.ObterPorIdAsync(comando.CategoriaId, comando.UsuarioId);

        if (categoria is null)
            TransacaoValidador.AdicionarErro(erros, nameof(comando.CategoriaId), "Categoria não encontrada.");

        if (erros.Count > 0)
            return ResultadoCadastroTransacao.ComDadosInvalidos(erros);

        var transacao = new Transacao(
            Guid.NewGuid(),
            comando.UsuarioId,
            comando.CategoriaId,
            comando.Valor,
            comando.DataOcorrencia,
            comando.Descricao,
            comando.Observacoes);

        await transacaoRepository.InserirAsync(transacao);

        return ResultadoCadastroTransacao.ComSucesso(transacao.Id);
    }
}
