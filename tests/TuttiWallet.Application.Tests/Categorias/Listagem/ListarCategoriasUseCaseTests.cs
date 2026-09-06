using FluentAssertions;
using Moq;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Listagem;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Categorias.Listagem;

public class ListarCategoriasUseCaseTests
{
    private readonly Mock<ICategoriaRepository> _categoriaRepository = new();
    private readonly ListarCategoriasUseCase _useCase;
    private readonly Guid _usuarioId = Guid.NewGuid();

    public ListarCategoriasUseCaseTests()
    {
        _useCase = new ListarCategoriasUseCase(_categoriaRepository.Object);
    }

    [Fact]
    public async Task ListarComDadosValidosRetornaSucessoComCategoriasETotal()
    {
        var categorias = new[]
        {
            new Categoria(Guid.NewGuid(), _usuarioId, "Alimentação", TipoTransacao.Despesa),
            new Categoria(Guid.NewGuid(), _usuarioId, "Salário", TipoTransacao.Receita)
        };
        _categoriaRepository.Setup(r => r.ObterQuantidadeAsync(_usuarioId)).ReturnsAsync(42);
        _categoriaRepository.Setup(r => r.ObterPaginadoAsync(_usuarioId, 2, 10)).ReturnsAsync(categorias);

        var resultado = await _useCase.ExecutarAsync(Comando(pagina: 2, tamanhoPagina: 10));

        resultado.Status.Should().Be(StatusListagemCategorias.Sucesso);
        resultado.TotalRegistros.Should().Be(42);
        resultado.Categorias.Should().BeEquivalentTo(categorias);
    }

    [Fact]
    public async Task ListarComPaginaInvalidaRetornaDadosInvalidosSemConsultarRepositorio()
    {
        var resultado = await _useCase.ExecutarAsync(Comando(pagina: 0));

        resultado.Status.Should().Be(StatusListagemCategorias.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(ListarCategoriasComando.Pagina));
        _categoriaRepository.Verify(r => r.ObterPaginadoAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _categoriaRepository.Verify(r => r.ObterQuantidadeAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ListarComTamanhoPaginaInvalidoRetornaDadosInvalidosSemConsultarRepositorio()
    {
        var resultado = await _useCase.ExecutarAsync(Comando(tamanhoPagina: 500));

        resultado.Status.Should().Be(StatusListagemCategorias.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(ListarCategoriasComando.TamanhoPagina));
        _categoriaRepository.Verify(r => r.ObterPaginadoAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    private ListarCategoriasComando Comando(int pagina = 1, int tamanhoPagina = 20) => new()
    {
        UsuarioId = _usuarioId,
        Pagina = pagina,
        TamanhoPagina = tamanhoPagina
    };
}
