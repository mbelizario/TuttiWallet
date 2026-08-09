using FluentAssertions;
using TuttiWallet.Domain;

namespace TuttiWallet.Domain.Tests;

public class CategoriaTests
{
    [Fact]
    public void CriarSubcategoriaAoInformarCategoriaPai()
    {
        var categoriaPaiId = Guid.NewGuid();

        var subcategoria = new Categoria(Guid.NewGuid(), Guid.NewGuid(), "Streaming", TipoTransacao.Despesa, categoriaPaiId);

        subcategoria.CategoriaPaiId.Should().Be(categoriaPaiId);
    }

    [Fact]
    public void LancarExcecaoAoDefinirCategoriaComoPaiDeSiMesma()
    {
        var id = Guid.NewGuid();

        var act = () => new Categoria(id, Guid.NewGuid(), "Lazer", TipoTransacao.Despesa, id);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LancarExcecaoQuandoNomeNaoInformado()
    {
        var act = () => new Categoria(Guid.NewGuid(), Guid.NewGuid(), "  ", TipoTransacao.Receita);

        act.Should().Throw<ArgumentException>();
    }
}
