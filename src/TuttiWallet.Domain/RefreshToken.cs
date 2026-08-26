namespace TuttiWallet.Domain;

public class RefreshToken
{
    public Guid Id { get; }
    public Guid UsuarioId { get; }
    public string HashToken { get; }
    public DateTime CriadoEm { get; }
    public DateTime ExpiraEm { get; }

    public RefreshToken(Guid id, Guid usuarioId, string hashToken, DateTime criadoEm, DateTime expiraEm)
    {
        if (string.IsNullOrWhiteSpace(hashToken))
        {
            throw new ArgumentException("O hash do token é obrigatório.", nameof(hashToken));
        }

        if (expiraEm <= criadoEm)
        {
            throw new ArgumentException("A data de expiração deve ser posterior à data de criação.", nameof(expiraEm));
        }

        Id = id;
        UsuarioId = usuarioId;
        HashToken = hashToken;
        CriadoEm = criadoEm;
        ExpiraEm = expiraEm;
    }
}
