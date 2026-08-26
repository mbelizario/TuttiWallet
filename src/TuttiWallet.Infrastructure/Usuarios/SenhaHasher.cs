using Microsoft.AspNetCore.Identity;
using TuttiWallet.Application.Usuarios;

namespace TuttiWallet.Infrastructure.Usuarios;

public sealed class SenhaHasher : ISenhaHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string GerarHash(string senha) => _passwordHasher.HashPassword(null!, senha);

    public bool VerificarHash(string hashSenha, string senhaFornecida)
    {
        var resultado = _passwordHasher.VerifyHashedPassword(null!, hashSenha, senhaFornecida);

        return resultado is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
