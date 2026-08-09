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
            TipoTransacao.Despesa,
            valor: 150.75m,
            dataOcorrencia: new DateOnly(2026, 8, 8));

        transacao.Valor.Should().Be(150.75m);
        transacao.Tipo.Should().Be(TipoTransacao.Despesa);
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
            TipoTransacao.Receita,
            valor,
            new DateOnly(2026, 8, 8));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
