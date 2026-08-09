namespace TuttiWallet.Domain;

public class User
{
    public Guid Id { get; }
    public string Email { get; }
    public string PasswordHash { get; }

    public User(Guid id, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O email é obrigatório.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("O hash de senha é obrigatório.", nameof(passwordHash));
        }

        Id = id;
        Email = email;
        PasswordHash = passwordHash;
    }
}
