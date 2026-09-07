using FluentAssertions;
using TuttiWallet.Domain;

namespace TuttiWallet.Domain.Tests;

public class TransacaoTests
{
    [Fact]
    public void CriarTransacaoComValorValido()
    {
        var transacao = new Transacao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            valor: 150.75m,
            dataOcorrencia: new DateOnly(2026, 8, 8),
            descricao: "Supermercado");

        transacao.Valor.Should().Be(150.75m);
        transacao.Descricao.Should().Be("Supermercado");
        transacao.Observacoes.Should().BeNull();
    }

    [Fact]
    public void CriarTransacaoComObservacoes()
    {
        var transacao = new Transacao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            valor: 100m,
            dataOcorrencia: new DateOnly(2026, 8, 8),
            descricao: "Salário",
            observacoes: "Referente a agosto");

        transacao.Observacoes.Should().Be("Referente a agosto");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void LancarExcecaoQuandoValorNaoEhPositivo(decimal valor)
    {
        var act = () => new Transacao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            valor,
            new DateOnly(2026, 8, 8),
            descricao: "Salário");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void LancarExcecaoQuandoDescricaoNaoInformada(string descricao)
    {
        var act = () => new Transacao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            valor: 100m,
            new DateOnly(2026, 8, 8),
            descricao);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LancarExcecaoQuandoDescricaoExcedeCemCaracteres()
    {
        var descricao = new string('A', 101);

        var act = () => new Transacao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            valor: 100m,
            new DateOnly(2026, 8, 8),
            descricao);

        act.Should().Throw<ArgumentException>();
    }
}
