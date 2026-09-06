using FluentAssertions;
using Moq;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Application.Usuarios.Cadastro;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Tests.Usuarios.Cadastro;

public class CadastrarPrimeiroUsuarioUseCaseTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<ISenhaHasher> _senhaHasher = new();
    private readonly CadastrarPrimeiroUsuarioUseCase _useCase;

    public CadastrarPrimeiroUsuarioUseCaseTests()
    {
        _useCase = new CadastrarPrimeiroUsuarioUseCase(_usuarioRepository.Object, _senhaHasher.Object);
    }

    [Fact]
    public async Task RetornarUsuarioJaExisteQuandoJaHouverUsuarioCadastrado()
    {
        _usuarioRepository.Setup(r => r.ObterQuantidadeUsuariosAsync()).ReturnsAsync(1);

        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusCadastroPrimeiroUsuario.UsuarioJaExiste);
        _usuarioRepository.Verify(r => r.InserirAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task RetornarDadosInvalidosQuandoEmailJaCadastrado()
    {
        _usuarioRepository.Setup(r => r.ObterQuantidadeUsuariosAsync()).ReturnsAsync(0);
        _usuarioRepository.Setup(r => r.ObterExistePorEmailAsync(It.IsAny<string>())).ReturnsAsync(true);

        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusCadastroPrimeiroUsuario.DadosInvalidos);
        resultado.Erros.Should().ContainKey(nameof(CadastrarPrimeiroUsuarioComando.Email));
        _usuarioRepository.Verify(r => r.InserirAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarUsuarioComSucessoQuandoDadosValidosENenhumUsuarioExistente()
    {
        _usuarioRepository.Setup(r => r.ObterQuantidadeUsuariosAsync()).ReturnsAsync(0);
        _usuarioRepository.Setup(r => r.ObterExistePorEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
        _senhaHasher.Setup(h => h.GerarHash(It.IsAny<string>())).Returns("hash-gerado");

        var resultado = await _useCase.ExecutarAsync(ComandoValido());

        resultado.Status.Should().Be(StatusCadastroPrimeiroUsuario.Sucesso);
        resultado.UsuarioId.Should().NotBeNull();
        _usuarioRepository.Verify(
            r => r.InserirAsync(It.Is<Usuario>(u =>
                u.Nome == "Maria" &&
                u.Celular == "11987654321" &&
                u.HashSenha == "hash-gerado")),
            Times.Once);
    }

    private static CadastrarPrimeiroUsuarioComando ComandoValido() => new()
    {
        Nome = "Maria",
        Sobrenome = "Silva",
        Email = "maria@exemplo.com",
        Celular = "(11) 98765-4321",
        Senha = "abcX24",
        ConfirmacaoSenha = "abcX24"
    };
}
