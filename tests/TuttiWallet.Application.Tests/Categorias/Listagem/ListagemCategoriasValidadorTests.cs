using FluentAssertions;
using TuttiWallet.Application.Categorias.Listagem;

namespace TuttiWallet.Application.Tests.Categorias.Listagem;

public class ListagemCategoriasValidadorTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RetornarErroQuandoPaginaMenorQueUm(int pagina)
    {
        var erros = ListagemCategoriasValidador.Validar(Comando(pagina: pagina));

        erros.Should().ContainKey(nameof(ListarCategoriasComando.Pagina));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void RetornarErroQuandoTamanhoPaginaForaDoIntervaloPermitido(int tamanhoPagina)
    {
        var erros = ListagemCategoriasValidador.Validar(Comando(tamanhoPagina: tamanhoPagina));

        erros.Should().ContainKey(nameof(ListarCategoriasComando.TamanhoPagina));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void NaoRetornarErroQuandoTamanhoPaginaNosLimitesPermitidos(int tamanhoPagina)
    {
        var erros = ListagemCategoriasValidador.Validar(Comando(tamanhoPagina: tamanhoPagina));

        erros.Should().NotContainKey(nameof(ListarCategoriasComando.TamanhoPagina));
    }

    [Fact]
    public void NaoRetornarErroComComandoValido()
    {
        var erros = ListagemCategoriasValidador.Validar(Comando());

        erros.Should().BeEmpty();
    }

    private static ListarCategoriasComando Comando(int pagina = 1, int tamanhoPagina = 20) => new()
    {
        UsuarioId = Guid.NewGuid(),
        Pagina = pagina,
        TamanhoPagina = tamanhoPagina
    };
}
