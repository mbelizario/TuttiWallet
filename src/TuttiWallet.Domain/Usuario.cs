namespace TuttiWallet.Domain;

public class Usuario
{
    public Guid Id { get; }
    public string Email { get; }
    public string HashSenha { get; }

    public Usuario(Guid id, string email, string hashSenha)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O email é obrigatório.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(hashSenha))
        {
            throw new ArgumentException("O hash de senha é obrigatório.", nameof(hashSenha));
        }

        Id = id;
        Email = email;
        HashSenha = hashSenha;
    }
}
