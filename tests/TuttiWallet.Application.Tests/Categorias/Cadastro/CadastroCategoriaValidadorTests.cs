using FluentAssertions;
using TuttiWallet.Application.Categorias;
using TuttiWallet.Application.Categorias.Cadastro;

namespace TuttiWallet.Application.Tests.Categorias.Cadastro;

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

    private static CadastrarCategoriaComando ComandoValido(string nome = "Salário", int tipoId = 1) => new()
    {
        UsuarioId = Guid.NewGuid(),
        Nome = nome,
        TipoId = tipoId
    };
}
