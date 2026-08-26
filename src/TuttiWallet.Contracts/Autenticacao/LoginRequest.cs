namespace TuttiWallet.Contracts.Autenticacao;

public sealed class LoginRequest
{
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
