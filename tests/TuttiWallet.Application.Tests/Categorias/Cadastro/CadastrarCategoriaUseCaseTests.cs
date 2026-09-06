using FluentAssertions;
using Moq;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Cadastro;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Categorias.Cadastro;

public class CadastrarCategoriaUseCaseTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly CadastrarCategoriaUseCase _useCase;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public CadastrarCategoriaUseCaseTests()
    {
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync([]);

        _useCase = new CadastrarCategoriaUseCase(_categoriaRepository.Object);
    }

    [Fact]
    public async Task CadastrarCategoriaPrincipalComSucesso()
    {
        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusCadastroCategoria.Sucesso);
        _categoriaRepository.Verify(
            r => r.InserirAsync(It.Is<Categoria>(c => c.CategoriaPaiId == null && c.Tipo == TipoTransacao.Receita)),
            Times.Once);
    }

    [Fact]
    public async Task CadastrarSubcategoriaComSucessoQuandoPaiPrincipalDeMesmoTipo()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(tipoId: 2, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusCadastroCategoria.Sucesso);
        _categoriaRepository.Verify(
            r => r.InserirAsync(It.Is<Categoria>(c => c.CategoriaPaiId == categoriaPai.Id)),
            Times.Once);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoNomeNaoInformado()
    {
        var resultado = await _useCase.ExecutarAsync(ComandoValido(nome: " "));

        resultado.Status.Should().Be(StatusCadastroCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarCategoriaComando.Nome));
        _categoriaRepository.Verify(r => r.InserirAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaPaiNaoEncontrada()
    {
        var categoriaPaiId = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPaiId, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(categoriaPaiId: categoriaPaiId));

        resultado.Status.Should().Be(StatusCadastroCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarCategoriaComando.CategoriaPaiId));
        _categoriaRepository.Verify(r => r.InserirAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaPaiJaEhSubcategoria()
    {
        var categoriaAvo = new Categoria(Guid.NewGuid(), _usuarioId, "Casa", TipoTransacao.Despesa);
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Contas", TipoTransacao.Despesa, categoriaAvo.Id);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(tipoId: 2, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusCadastroCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarCategoriaComando.CategoriaPaiId));
        _categoriaRepository.Verify(r => r.InserirAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoTipoDivergeDoPai()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(tipoId: 1, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusCadastroCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarCategoriaComando.TipoId));
        _categoriaRepository.Verify(r => r.InserirAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoJaExisteCategoriaComMesmoNomeNormalizado()
    {
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(_usuarioId, null))
            .ReturnsAsync(["Fundos Imobiliários"]);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(nome: "Fundos Imobiliarios"));

        resultado.Status.Should().Be(StatusCadastroCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarCategoriaComando.Nome));
        _categoriaRepository.Verify(r => r.InserirAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task PermitirMesmoNomeDeSubcategoriaEmPaisDiferentes()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(_usuarioId, It.Is<Guid?>(id => id != categoriaPai.Id)))
            .ReturnsAsync(["Outros"]);
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(_usuarioId, categoriaPai.Id))
            .ReturnsAsync([]);

        var resultado = await _useCase.ExecutarAsync(ComandoValido(nome: "Outros", tipoId: 2, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusCadastroCategoria.Sucesso);
    }

    private CadastrarCategoriaComando ComandoValido(string nome = "Salário", int tipoId = 1, Guid? categoriaPaiId = null) => new()
    {
        UsuarioId = _usuarioId,
        Nome = nome,
        TipoId = tipoId,
        CategoriaPaiId = categoriaPaiId
    };
}
