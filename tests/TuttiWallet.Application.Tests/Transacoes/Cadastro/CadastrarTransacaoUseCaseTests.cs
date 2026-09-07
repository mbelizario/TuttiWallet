using FluentAssertions;
using Moq;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Transacoes;
using TuttiWallet.Application.Transacoes.Cadastro;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Transacoes.Cadastro;

public class CadastrarTransacaoUseCaseTests
{
    private readonly Mock<ITransacaoRepository> _transacaoRepository = new();
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly CadastrarTransacaoUseCase _useCase;
    private readonly Guid _usuarioId = Guid.NewGuid();
    private readonly Categoria _categoria;

    public CadastrarTransacaoUseCaseTests()
    {
        _categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(_categoria.Id, _usuarioId)).ReturnsAsync(_categoria);

        _useCase = new CadastrarTransacaoUseCase(_transacaoRepository.Object, _categoriaRepository.Object);
    }

    [Fact]
    public async Task CadastrarTransacaoComSucesso()
    {
        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusCadastroTransacao.Sucesso);
        resultado.TransacaoId.Should().NotBeNull();
        _transacaoRepository.Verify(
            r => r.InserirAsync(It.Is<Transacao>(t =>
                t.CategoriaId == _categoria.Id &&
                t.Descricao == "Supermercado")),
            Times.Once);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoDescricaoNaoInformada()
    {
        var resultado = await _useCase.ExecutarAsync(ComandoValido(descricao: " "));

        resultado.Status.Should().Be(StatusCadastroTransacao.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarTransacaoComando.Descricao));
        _transacaoRepository.Verify(r => r.InserirAsync(It.IsAny<Transacao>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaNaoEncontrada()
    {
        var categoriaId = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaId, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(categoriaId: categoriaId));

        resultado.Status.Should().Be(StatusCadastroTransacao.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarTransacaoComando.CategoriaId));
        _transacaoRepository.Verify(r => r.InserirAsync(It.IsAny<Transacao>()), Times.Never);
    }

    private CadastrarTransacaoComando ComandoValido(
        string descricao = "Supermercado", Guid? categoriaId = null) => new()
    {
        UsuarioId = _usuarioId,
        CategoriaId = categoriaId ?? _categoria.Id,
        Valor = 150.75m,
        DataOcorrencia = new DateOnly(2026, 8, 8),
        Descricao = descricao
    };
}
