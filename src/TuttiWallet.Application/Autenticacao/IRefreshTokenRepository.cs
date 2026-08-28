using TuttiWallet.Domain;

namespace TuttiWallet.Application.Autenticacao;

public interface IRefreshTokenRepository
{
    Task InserirAsync(RefreshToken refreshToken);
}
