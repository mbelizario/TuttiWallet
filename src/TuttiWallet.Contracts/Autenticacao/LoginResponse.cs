namespace TuttiWallet.Contracts.Autenticacao;

public sealed class LoginResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime ExpiraEm { get; init; }
}
