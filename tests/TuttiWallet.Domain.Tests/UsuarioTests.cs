using FluentAssertions;
using TuttiWallet.Domain;

namespace TuttiWallet.Domain.Tests;

public class UsuarioTests
{
    [Fact]
    public void CriarUsuarioComDadosValidos()
    {
        var id = Guid.NewGuid();

        var usuario = new Usuario(id, "Maria", "Silva", "maria@exemplo.com", "11987654321", "hash-de-senha");

        usuario.Id.Should().Be(id);
        usuario.Nome.Should().Be("Maria");
        usuario.Sobrenome.Should().Be("Silva");
        usuario.Email.Should().Be("maria@exemplo.com");
        usuario.Celular.Should().Be("11987654321");
        usuario.HashSenha.Should().Be("hash-de-senha");
    }

    [Theory]
    [InlineData("", "Silva", "maria@exemplo.com", "11987654321", "hash")]
    [InlineData("Maria", "", "maria@exemplo.com", "11987654321", "hash")]
    [InlineData("Maria", "Silva", "", "11987654321", "hash")]
    [InlineData("Maria", "Silva", "maria@exemplo.com", "", "hash")]
    [InlineData("Maria", "Silva", "maria@exemplo.com", "11987654321", "")]
    public void LancarExcecaoQuandoCampoObrigatorioNaoInformado(
        string nome, string sobrenome, string email, string celular, string hashSenha)
    {
        var act = () => new Usuario(Guid.NewGuid(), nome, sobrenome, email, celular, hashSenha);

        act.Should().Throw<ArgumentException>();
    }
}
