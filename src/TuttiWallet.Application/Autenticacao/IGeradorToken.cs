using TuttiWallet.Domain;

namespace TuttiWallet.Application.Autenticacao;

public interface IGeradorToken
{
    (string Token, DateTime ExpiraEm) GerarTokenAcesso(Usuario usuario);

    (string TokenBruto, string HashToken, DateTime ExpiraEm) GerarTokenRenovacao();
}
