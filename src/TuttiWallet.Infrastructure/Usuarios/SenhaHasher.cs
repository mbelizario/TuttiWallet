using Microsoft.AspNetCore.Identity;
using TuttiWallet.Application.Usuarios;

namespace TuttiWallet.Infrastructure.Usuarios;

public sealed class SenhaHasher : ISenhaHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string GerarHash(string senha) => _passwordHasher.HashPassword(null!, senha);
}
