using TuttiWallet.Application.Usuarios;
using TuttiWallet.Domain;

namespace TuttiWallet.Application.Autenticacao;

public sealed class AutenticarUsuarioUseCase(
    IUsuarioRepository usuarioRepository,
    ISenhaHasher senhaHasher,
    IRefreshTokenRepository refreshTokenRepository,
    IGeradorToken geradorToken)
{
    public async Task<ResultadoAutenticacao> ExecutarAsync(AutenticarUsuarioComando comando)
    {
        var usuario = await usuarioRepository.ObterPorEmailAsync(comando.Email);

        if (usuario is null || !senhaHasher.VerificarHash(usuario.HashSenha, comando.Senha))
        {
            return ResultadoAutenticacao.ComCredenciaisInvalidas();
        }

        var (accessToken, expiraEmAcesso) = geradorToken.GerarTokenAcesso(usuario);
        var (tokenRenovacaoBruto, hashTokenRenovacao, expiraEmRenovacao) = geradorToken.GerarTokenRenovacao();

        var refreshToken = new RefreshToken(Guid.NewGuid(), usuario.Id, hashTokenRenovacao, DateTime.UtcNow, expiraEmRenovacao);
        await refreshTokenRepository.InserirAsync(refreshToken);

        return ResultadoAutenticacao.ComSucesso(accessToken, tokenRenovacaoBruto, expiraEmAcesso);
    }
}
