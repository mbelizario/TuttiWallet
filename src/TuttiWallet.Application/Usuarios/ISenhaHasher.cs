namespace TuttiWallet.Application.Usuarios;

public interface ISenhaHasher
{
    string GerarHash(string senha);

    bool VerificarHash(string hashSenha, string senhaFornecida);
}
