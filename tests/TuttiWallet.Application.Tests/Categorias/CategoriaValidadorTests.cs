using FluentAssertions;
using TuttiWallet.Application.Categorias;

namespace TuttiWallet.Application.Tests.Categorias;

public class CategoriaValidadorTests
{
    [Theory]
    [InlineData("Fundos Imobiliários", "Fundos Imobiliarios")]
    [InlineData("Alimentação", "  Alimentacao!  ")]
    [InlineData("Contas de Casa", "Contas   de    Casa")]
    public void NormalizarNomesEquivalentesParaMesmoResultado(string nome, string nomeEquivalente)
    {
        CategoriaValidador.NormalizarNome(nome)
            .Should().Be(CategoriaValidador.NormalizarNome(nomeEquivalente));
    }
}
