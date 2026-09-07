using FluentAssertions;
using Moq;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Exclusao;
using TuttiWallet.Application.Transacoes;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Categorias.Exclusao;

public class ExcluirCategoriaUseCaseTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<ITransacaoRepository> _transacaoRepository = new();
    private readonly ExcluirCategoriaUseCase _useCase;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public ExcluirCategoriaUseCaseTests()
    {
        _categoriaRepository
            .Setup(r => r.ObterSubcategoriasAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync([]);
        _transacaoRepository
            .Setup(r => r.ExisteTransacaoParaCategoriaAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        _useCase = new ExcluirCategoriaUseCase(_categoriaRepository.Object, _transacaoRepository.Object);
    }

    [Fact]
    public async Task ExcluirCategoriaComSucesso()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id));

        resultado.Status.Should().Be(StatusExclusaoCategoria.Sucesso);
        _categoriaRepository.Verify(r => r.ExcluirAsync(categoria.Id, _usuarioId), Times.Once);
    }

    [Fact]
    public async Task RetornarNaoEncontradaQuandoCategoriaNaoExiste()
    {
        var categoriaId = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaId, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(Comando(categoriaId));

        resultado.Status.Should().Be(StatusExclusaoCategoria.NaoEncontrada);
        _categoriaRepository.Verify(r => r.ExcluirAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaPrincipalPossuiSubcategorias()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository
            .Setup(r => r.ObterSubcategoriasAsync(_usuarioId, categoria.Id))
            .ReturnsAsync([new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa, categoria.Id)]);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id));

        resultado.Status.Should().Be(StatusExclusaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(ExcluirCategoriaComando.CategoriaId));
        _categoriaRepository.Verify(r => r.ExcluirAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaPossuiTransacoesVinculadas()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _transacaoRepository.Setup(r => r.ExisteTransacaoParaCategoriaAsync(categoria.Id)).ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id));

        resultado.Status.Should().Be(StatusExclusaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(ExcluirCategoriaComando.CategoriaId));
        _categoriaRepository.Verify(r => r.ExcluirAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task PermitirExcluirSubcategoriaSemVerificarSubcategoriasDaPropriaSubcategoria()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        var subcategoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa, categoriaPai.Id);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(subcategoria.Id, _usuarioId)).ReturnsAsync(subcategoria);

        var resultado = await _useCase.ExecutarAsync(Comando(subcategoria.Id));

        resultado.Status.Should().Be(StatusExclusaoCategoria.Sucesso);
        _categoriaRepository.Verify(r => r.ObterSubcategoriasAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    private ExcluirCategoriaComando Comando(Guid categoriaId) => new()
    {
        UsuarioId = _usuarioId,
        CategoriaId = categoriaId
    };
}
