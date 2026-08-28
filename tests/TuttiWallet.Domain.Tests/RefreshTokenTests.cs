using FluentAssertions;
using TuttiWallet.Domain;

namespace TuttiWallet.Domain.Tests;

public class RefreshTokenTests
{
    [Fact]
    public void CriarRefreshTokenComDadosValidos()
    {
        var id = Guid.NewGuid();
        var usuarioId = Guid.NewGuid();
        var criadoEm = DateTime.UtcNow;
        var expiraEm = criadoEm.AddDays(7);

        var refreshToken = new RefreshToken(id, usuarioId, "hash-do-token", criadoEm, expiraEm);

        refreshToken.Id.Should().Be(id);
        refreshToken.UsuarioId.Should().Be(usuarioId);
        refreshToken.HashToken.Should().Be("hash-do-token");
        refreshToken.CriadoEm.Should().Be(criadoEm);
        refreshToken.ExpiraEm.Should().Be(expiraEm);
    }

    [Fact]
    public void LancarExcecaoQuandoHashTokenNaoInformado()
    {
        var act = () => new RefreshToken(Guid.NewGuid(), Guid.NewGuid(), "", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void LancarExcecaoQuandoExpiraEmNaoForPosteriorACriadoEm()
    {
        var criadoEm = DateTime.UtcNow;

        var act = () => new RefreshToken(Guid.NewGuid(), Guid.NewGuid(), "hash-do-token", criadoEm, criadoEm);

        act.Should().Throw<ArgumentException>();
    }
}
