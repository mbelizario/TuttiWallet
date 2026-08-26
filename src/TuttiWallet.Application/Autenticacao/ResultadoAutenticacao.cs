namespace TuttiWallet.Application.Autenticacao;

public sealed class ResultadoAutenticacao
{
    public StatusAutenticacao Status { get; }
    public string? AccessToken { get; }
    public string? RefreshToken { get; }
    public DateTime? ExpiraEm { get; }

    private ResultadoAutenticacao(StatusAutenticacao status, string? accessToken, string? refreshToken, DateTime? expiraEm)
    {
        Status = status;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiraEm = expiraEm;
    }

    public static ResultadoAutenticacao ComSucesso(string accessToken, string refreshToken, DateTime expiraEm) =>
        new(StatusAutenticacao.Sucesso, accessToken, refreshToken, expiraEm);

    public static ResultadoAutenticacao ComCredenciaisInvalidas() =>
        new(StatusAutenticacao.CredenciaisInvalidas, null, null, null);
}
