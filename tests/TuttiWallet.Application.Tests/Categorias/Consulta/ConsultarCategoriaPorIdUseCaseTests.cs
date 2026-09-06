using FluentAssertions;
using Moq;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Consulta;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Categorias.Consulta;

public class ConsultarCategoriaPorIdUseCaseTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly ConsultarCategoriaPorIdUseCase _useCase;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public ConsultarCategoriaPorIdUseCaseTests()
    {
        _useCase = new ConsultarCategoriaPorIdUseCase(_categoriaRepository.Object);
    }

    [Fact]
    public async Task ConsultarCategoriaPrincipalSemSubcategoriasRetornaSucesso()
    {
        var categoria = new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoria.Id, _usuarioId)).ReturnsAsync(categoria);
        _categoriaRepository.Setup(r => r.ObterSubcategoriasAsync(_usuarioId, categoria.Id)).ReturnsAsync([]);

        var resultado = await _useCase.ExecutarAsync(Comando(categoria.Id));

        resultado.Status.Should().Be(StatusConsultaCategoriaPorId.Sucesso);
        resultado.Categoria.Should().Be(categoria);
        resultado.Subcategorias.Should().BeEmpty();
    }

    [Fact]
    public async Task ConsultarCategoriaPrincipalComSubcategoriasRetornaSucessoComListaPreenchida()
    {
        var categoriaPrincipal = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        var subcategoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa, categoriaPrincipal.Id);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaPrincipal.Id, _usuarioId)).ReturnsAsync(categoriaPrincipal);
        _categoriaRepository.Setup(r => r.ObterSubcategoriasAsync(_usuarioId, categoriaPrincipal.Id)).ReturnsAsync([subcategoria]);

        var resultado = await _useCase.ExecutarAsync(Comando(categoriaPrincipal.Id));

        resultado.Status.Should().Be(StatusConsultaCategoriaPorId.Sucesso);
        resultado.Subcategorias.Should().ContainSingle().Which.Should().Be(subcategoria);
    }

    [Fact]
    public async Task ConsultarSubcategoriaRetornaSucessoSemBuscarSubcategorias()
    {
        var categoriaPrincipal = new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa);
        var subcategoria = new Categoria(Guid.NewGuid(), _usuarioId, "Restaurante", TipoTransacao.Despesa, categoriaPrincipal.Id);
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(subcategoria.Id, _usuarioId)).ReturnsAsync(subcategoria);

        var resultado = await _useCase.ExecutarAsync(Comando(subcategoria.Id));

        resultado.Status.Should().Be(StatusConsultaCategoriaPorId.Sucesso);
        resultado.Categoria.Should().Be(subcategoria);
        resultado.Subcategorias.Should().BeEmpty();
        _categoriaRepository.Verify(r => r.ObterSubcategoriasAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ConsultarCategoriaInexistenteRetornaNaoEncontrada()
    {
        var categoriaId = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaId, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(Comando(categoriaId));

        resultado.Status.Should().Be(StatusConsultaCategoriaPorId.NaoEncontrada);
        resultado.Categoria.Should().BeNull();
    }

    [Fact]
    public async Task ConsultarCategoriaDeOutroUsuarioRetornaNaoEncontrada()
    {
        var categoriaDeOutroUsuario = Guid.NewGuid();
        _categoriaRepository.Setup(r => r.ObterPorIdAsync(categoriaDeOutroUsuario, _usuarioId)).ReturnsAsync((Categoria?)null);

        var resultado = await _useCase.ExecutarAsync(Comando(categoriaDeOutroUsuario));

        resultado.Status.Should().Be(StatusConsultaCategoriaPorId.NaoEncontrada);
    }

    private ConsultarCategoriaPorIdComando Comando(Guid categoriaId) => new()
    {
        UsuarioId = _usuarioId,
        CategoriaId = categoriaId
    };
}
