using FluentAssertions;
using Moq;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Autenticacao;

public class AutenticarUsuarioUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<ISenhaHasher> _senhaHasher = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
    private readonly Mock<IGeradorToken> _geradorToken = new();
    private readonly AutenticarUsuarioUseCase _useCase;

    public AutenticarUsuarioUseCaseTests()
    {
        _useCase = new AutenticarUsuarioUseCase(
            _usuarioRepository.Object,
            _senhaHasher.Object,
            _refreshTokenRepository.Object,
            _geradorToken.Object);
    }

    [Fact]
    public async Task RetornarCredenciaisInvalidasQuandoUsuarioNaoEncontrado()
    {
        _usuarioRepository.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ReturnsAsync((Usuario?)null);

        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusAutenticacao.CredenciaisInvalidas);
        _refreshTokenRepository.Verify(r => r.InserirAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task RetornarCredenciaisInvalidasQuandoSenhaNaoConfere()
    {
        _usuarioRepository.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ReturnsAsync(UsuarioValido());
        _senhaHasher.Setup(h => h.VerificarHash(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusAutenticacao.CredenciaisInvalidas);
        _refreshTokenRepository.Verify(r => r.InserirAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    [Fact]
    public async Task AutenticarComSucessoQuandoCredenciaisValidas()
    {
        var usuario = UsuarioValido();
        var expiraEmAcesso = DateTime.UtcNow.AddMinutes(15);
        var expiraEmRenovacao = DateTime.UtcNow.AddDays(7);

        _usuarioRepository.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
        _senhaHasher.Setup(h => h.VerificarHash(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _geradorToken.Setup(g => g.GerarTokenAcesso(usuario)).Returns(("access-token", expiraEmAcesso));
        _geradorToken.Setup(g => g.GerarTokenRenovacao()).Returns(("token-bruto", "hash-token", expiraEmRenovacao));

        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusAutenticacao.Sucesso);
        resultado.AccessToken.Should().Be("access-token");
        resultado.RefreshToken.Should().Be("token-bruto");
        resultado.ExpiraEm.Should().Be(expiraEmAcesso);
        _refreshTokenRepository.Verify(
            r => r.InserirAsync(It.Is<RefreshToken>(t =>
                t.UsuarioId == usuario.Id &&
                t.HashToken == "hash-token" &&
                t.ExpiraEm == expiraEmRenovacao)),
            Times.Once);
    }

    private static Usuario UsuarioValido() => new(
        Guid.NewGuid(), "Maria", "Silva", "maria@exemplo.com", "11987654321", "hash-de-senha");

    private static AutenticarUsuarioComando ComandoValido() => new()
    {
        Email = "maria@exemplo.com",
        Senha = "abcX24"
    };
}
