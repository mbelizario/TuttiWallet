namespace TuttiWallet.Application.Autenticacao;

public sealed class AutenticarUsuarioComando
{
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
