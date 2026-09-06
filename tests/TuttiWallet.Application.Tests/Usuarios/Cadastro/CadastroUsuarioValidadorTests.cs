using FluentAssertions;
using TuttiWallet.Application.Usuarios;
using TuttiWallet.Application.Usuarios.Cadastro;

namespace TuttiWallet.Application.Tests.Usuarios.Cadastro;

public class CadastroUsuarioValidadorTests
{
    [Fact]
    public void NaoRetornarErrosQuandoComandoValido()
    {
        var comando = ComandoValido();

        var erros = CadastroUsuarioValidador.Validar(comando);

        erros.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Maria2")]
    [InlineData("M")]
    public void RetornarErroDeNomeQuandoFormatoOuTamanhoInvalido(string nome)
    {
        var comando = ComandoValido(nome: nome);

        var erros = CadastroUsuarioValidador.Validar(comando);

        erros.Should().ContainKey(nameof(CadastrarPrimeiroUsuarioComando.Nome));
    }

    [Theory]
    [InlineData("email-invalido")]
    [InlineData("email@")]
    public void RetornarErroDeEmailQuandoFormatoInvalido(string email)
    {
        var comando = ComandoValido(email: email);

        var erros = CadastroUsuarioValidador.Validar(comando);

        erros.Should().ContainKey(nameof(CadastrarPrimeiroUsuarioComando.Email));
    }

    [Theory]
    [InlineData("11987654321")]
    [InlineData("(11) 9876-5432")]
    public void RetornarErroDeCelularQuandoForaDoFormatoEsperado(string celular)
    {
        var comando = ComandoValido(celular: celular);

        var erros = CadastroUsuarioValidador.Validar(comando);

        erros.Should().ContainKey(nameof(CadastrarPrimeiroUsuarioComando.Celular));
    }

    [Theory]
    [InlineData("abcdef")]
    [InlineData("123456")]
    [InlineData("abc123xy")]
    [InlineData("ab1")]
    public void RetornarErroDeSenhaQuandoRegrasNaoAtendidas(string senha)
    {
        var comando = ComandoValido(senha: senha, confirmacaoSenha: senha);

        var erros = CadastroUsuarioValidador.Validar(comando);

        erros.Should().ContainKey(nameof(CadastrarPrimeiroUsuarioComando.Senha));
    }

    [Fact]
    public void RetornarErroQuandoConfirmacaoSenhaDivergeDaSenha()
    {
        var comando = ComandoValido(senha: "abc123", confirmacaoSenha: "abc124");

        var erros = CadastroUsuarioValidador.Validar(comando);

        erros.Should().ContainKey(nameof(CadastrarPrimeiroUsuarioComando.ConfirmacaoSenha));
    }

    private static CadastrarPrimeiroUsuarioComando ComandoValido(
        string nome = "Maria",
        string sobrenome = "Silva",
        string email = "maria@exemplo.com",
        string celular = "(11) 98765-4321",
        string senha = "abcX24",
        string? confirmacaoSenha = null) =>
        new()
        {
            Nome = nome,
            Sobrenome = sobrenome,
            Email = email,
            Celular = celular,
            Senha = senha,
            ConfirmacaoSenha = confirmacaoSenha ?? senha
        };
}
