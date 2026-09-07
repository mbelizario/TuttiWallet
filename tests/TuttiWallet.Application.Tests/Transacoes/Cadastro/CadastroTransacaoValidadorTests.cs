using FluentAssertions;
using TuttiWallet.Application.Transacoes.Cadastro;

namespace TuttiWallet.Application.Tests.Transacoes.Cadastro;

public class CadastroTransacaoValidadorTests
{
    [Fact]
    public void RetornarErroQuandoDescricaoNaoInformada()
    {
        var erros = CadastroTransacaoValidador.Validar(ComandoValido(descricao: "  "));

        erros.Should().ContainKey(nameof(CadastrarTransacaoComando.Descricao));
    }

    [Fact]
    public void RetornarErroQuandoDescricaoExcedeCemCaracteres()
    {
        var erros = CadastroTransacaoValidador.Validar(ComandoValido(descricao: new string('A', 101)));

        erros.Should().ContainKey(nameof(CadastrarTransacaoComando.Descricao));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void RetornarErroQuandoValorNaoEhPositivo(decimal valor)
    {
        var erros = CadastroTransacaoValidador.Validar(ComandoValido(valor: valor));

        erros.Should().ContainKey(nameof(CadastrarTransacaoComando.Valor));
    }

    [Fact]
    public void NaoRetornarErrosQuandoDadosValidos()
    {
        var erros = CadastroTransacaoValidador.Validar(ComandoValido());

        erros.Should().BeEmpty();
    }

    private static CadastrarTransacaoComando ComandoValido(
        string descricao = "Supermercado", decimal valor = 100m) => new()
    {
        UsuarioId = Guid.NewGuid(),
        CategoriaId = Guid.NewGuid(),
        Valor = valor,
        DataOcorrencia = new DateOnly(2026, 8, 8),
        Descricao = descricao
    };
}
