using FluentAssertions;
using TuttiWallet.Application.Categorias.Edicao;

namespace TuttiWallet.Application.Tests.Categorias.Edicao;

public class EdicaoCategoriaValidadorTests
{
    [Fact]
    public void RetornarErroQuandoNomeNaoInformado()
    {
        var erros = EdicaoCategoriaValidador.Validar(ComandoValido(nome: "  "));

        erros.Should().ContainKey(nameof(EditarCategoriaComando.Nome));
    }

    [Fact]
    public void RetornarErroQuandoTipoIdInvalido()
    {
        var erros = EdicaoCategoriaValidador.Validar(ComandoValido(tipoId: 99));

        erros.Should().ContainKey(nameof(EditarCategoriaComando.TipoId));
    }

    [Fact]
    public void NaoRetornarErrosQuandoDadosValidos()
    {
        var erros = EdicaoCategoriaValidador.Validar(ComandoValido());

        erros.Should().BeEmpty();
    }

    private static EditarCategoriaComando ComandoValido(string nome = "Salário", int tipoId = 1) => new()
    {
        UsuarioId = Guid.NewGuid(),
        CategoriaId = Guid.NewGuid(),
        Nome = nome,
        TipoId = tipoId
    };
}
