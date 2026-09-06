using FluentAssertions;
using TuttiWallet.Application.Categorias;

namespace TuttiWallet.Application.Tests.Categorias;

public class CadastroCategoriaValidadorTests
{
    [Fact]
    public void RetornarErroQuandoNomeNaoInformado()
    {
        var erros = CadastroCategoriaValidador.Validar(ComandoValido(nome: "  "));

        erros.Should().ContainKey(nameof(CadastrarCategoriaComando.Nome));
    }

    [Fact]
    public void RetornarErroQuandoTipoIdInvalido()
    {
        var erros = CadastroCategoriaValidador.Validar(ComandoValido(tipoId: 99));

        erros.Should().ContainKey(nameof(CadastrarCategoriaComando.TipoId));
    }

    [Fact]
    public void NaoRetornarErrosQuandoDadosValidos()
    {
        var erros = CadastroCategoriaValidador.Validar(ComandoValido());

        erros.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Fundos Imobiliários", "Fundos Imobiliarios")]
    [InlineData("Alimentação", "  Alimentacao!  ")]
    [InlineData("Contas de Casa", "Contas   de    Casa")]
    public void NormalizarNomesEquivalentesParaMesmoResultado(string nome, string nomeEquivalente)
    {
        CadastroCategoriaValidador.NormalizarNome(nome)
            .Should().Be(CadastroCategoriaValidador.NormalizarNome(nomeEquivalente));
    }

    private static CadastrarCategoriaComando ComandoValido(string nome = "Salário", int tipoId = 1) => new()
    {
        UsuarioId = Guid.NewGuid(),
        Nome = nome,
        TipoId = tipoId
    };
}
