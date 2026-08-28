namespace TuttiWallet.Infrastructure.Autenticacao;

public sealed class JwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Chave { get; init; }
    public required int MinutosExpiracaoTokenAcesso { get; init; }
    public required int DiasExpiracaoTokenRenovacao { get; init; }
}
