using FluentAssertions;
using Moq;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Edicao;
using TuttiWallet.Application.Transacoes;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Categorias.Edicao;

public class EditarCategoriaUseCaseTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly Mock<ITransacaoRepository> _transacaoRepository = new();
    private readonly EditarCategoriaUseCase _useCase;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public EditarCategoriaUseCaseTests()
    {
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(It.IsAny<Guid>(), It.IsAny<Guid?>()))
            .ReturnsAsync([]);
        _categoriaRepository
            .Setup(r => r.ObterSubcategoriasAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync([]);
        _transacaoRepository
            .Setup(r => r.ExisteTransacaoParaCategoriaAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        _useCase = new EditarCategoriaUseCase(_categoriaRepository.Object, _transacaoRepository.Object);
    }

    [Fact]
    public async Task EditarCategoriaComSucesso()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, nome: "Salário CLT"));

        resultado.Status.Should().Be(StatusEdicaoCategoria.Sucesso);
        _categoriaRepository.Verify(
            r => r.AtualizarAsync(It.Is<Categoria>(c => c.Id == categoria.Id && c.Nome == "Salário CLT")),
            Times.Once);
    }

    [Fact]
    public async Task RetornarNaoEncontradaQuandoCategoriaNaoExiste()
    {
        var categoriaId = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaId, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(Comando(categoriaId));

        resultado.Status.Should().Be(StatusEdicaoCategoria.NaoEncontrada);
        _categoriaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoNomeNaoInformado()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, nome: " "));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.Nome));
        _categoriaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoDefinePaiComoSiMesma()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, categoriaPaiId: categoria.Id));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.CategoriaPaiId));
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaPaiNaoEncontrada()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa);
        var categoriaPaiId = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPaiId, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, categoriaPaiId: categoriaPaiId));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.CategoriaPaiId));
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaPaiJaEhSubcategoria()
    {
        var categoriaAvo = new Categoria(Guid.NewGuid(), _usuarioId, "Casa", TipoTransacao.Despesa);
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Contas", TipoTransacao.Despesa, categoriaAvo.Id);
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Água", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.CategoriaPaiId));
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoTipoDivergeDoPai()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, tipoId: 1, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.TipoId));
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoJaExisteCategoriaComMesmoNomeNormalizado()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(_usuarioId, null))
            .ReturnsAsync(["Salário", "Fundos Imobiliários"]);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, nome: "Fundos Imobiliarios"));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.Nome));
    }

    [Fact]
    public async Task PermitirManterNomeAtualAoEditarOutrosCampos()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository
            .Setup(r => r.ObterNomesPorPaiAsync(_usuarioId, null))
            .ReturnsAsync(["Salário"]);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, nome: "Salário"));

        resultado.Status.Should().Be(StatusEdicaoCategoria.Sucesso);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoCategoriaComSubcategoriasTentaVirarSubcategoria()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Lazer", TipoTransacao.Despesa);
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);
        _categoriaRepository
            .Setup(r => r.ObterSubcategoriasAsync(_usuarioId, categoria.Id))
            .ReturnsAsync([new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa, categoria.Id)]);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, tipoId: 2, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.CategoriaPaiId));
        _transacaoRepository.Verify(r => r.ExisteTransacaoParaCategoriaAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoAlterandoPaiDeCategoriaComTransacoes()
    {
        var categoriaPai = new Categoria(Guid.NewGuid(), _usuarioId, "Lazer", TipoTransacao.Despesa);
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPai.Id, _usuarioId)).ReturnsAsync(categoriaPai);
        _transacaoRepository.Setup(r => r.ExisteTransacaoParaCategoriaAsync(categoria.Id)).ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, tipoId: 2, categoriaPaiId: categoriaPai.Id));

        resultado.Status.Should().Be(StatusEdicaoCategoria.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(EditarCategoriaComando.CategoriaPaiId));
        _categoriaRepository.Verify(r => r.AtualizarAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task PermitirEditarCategoriaComTransacoesQuandoPaiNaoMuda()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _transacaoRepository.Setup(r => r.ExisteTransacaoParaCategoriaAsync(categoria.Id)).ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id, nome: "Restaurantes"));

        resultado.Status.Should().Be(StatusEdicaoCategoria.Sucesso);
        _transacaoRepository.Verify(r => r.ExisteTransacaoParaCategoriaAsync(It.IsAny<Guid>()), Times.Never);
    }

    private EditarCategoriaComando Comando(Guid categoriaId, string nome = "Salário", int tipoId = 1, Guid? categoriaPaiId = null) => new()
    {
        UsuarioId = _usuarioId,
        CategoriaId = categoriaId,
        Nome = nome,
        TipoId = tipoId,
        CategoriaPaiId = categoriaPaiId
    };
}
