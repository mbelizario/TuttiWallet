namespace TuttiWallet.Domain;

public class Usuario
{
    public Guid Id { get; }
    public string Nome { get; }
    public string Sobrenome { get; }
    public string Email { get; }
    public string Celular { get; }
    public string HashSenha { get; }

    public Usuario(Guid id, string nome, string sobrenome, string email, string celular, string hashSenha)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));
        }

        if (string.IsNullOrWhiteSpace(sobrenome))
        {
            throw new ArgumentException("O sobrenome é obrigatório.", nameof(sobrenome));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O email é obrigatório.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(celular))
        {
            throw new ArgumentException("O celular é obrigatório.", nameof(celular));
        }

        if (string.IsNullOrWhiteSpace(hashSenha))
        {
            throw new ArgumentException("O hash de senha é obrigatório.", nameof(hashSenha));
        }

        Id = id;
        Nome = nome;
        Sobrenome = sobrenome;
        Email = email;
        Celular = celular;
        HashSenha = hashSenha;
    }
}
