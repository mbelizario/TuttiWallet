namespace TuttiWallet.Application.Usuarios;

public interface ISenhaHasher
{
    string GerarHash(string senha);
}
