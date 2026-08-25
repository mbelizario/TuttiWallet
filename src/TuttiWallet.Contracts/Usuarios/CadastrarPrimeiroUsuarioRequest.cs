namespace TuttiWallet.Contracts.Usuarios;

public sealed class CadastrarPrimeiroUsuarioRequest
{
    public required string Nome { get; init; }
    public required string Sobrenome { get; init; }
    public required string Email { get; init; }
    public required string Celular { get; init; }
    public required string Senha { get; init; }
    public required string ConfirmacaoSenha { get; init; }
}
