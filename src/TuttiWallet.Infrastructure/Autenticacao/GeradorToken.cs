using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TuttiWallet.Application.Autenticacao;
using TuttiWallet.Domain;

namespace TuttiWallet.Infrastructure.Autenticacao;

public sealed class GeradorToken(IOptions<JwtOptions> opcoes) : IGeradorToken
{
    private readonly JwtOptions _opcoes = opcoes.Value;

    public (string Token, DateTime ExpiraEm) GerarTokenAcesso(Usuario usuario)
    {
        var expiraEm = DateTime.UtcNow.AddMinutes(_opcoes.MinutosExpiracaoTokenAcesso);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email)
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.Chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opcoes.Issuer,
            audience: _opcoes.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }

    public (string TokenBruto, string HashToken, DateTime ExpiraEm) GerarTokenRenovacao()
    {
        var tokenBruto = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenBruto)));
        var expiraEm = DateTime.UtcNow.AddDays(_opcoes.DiasExpiracaoTokenRenovacao);

        return (tokenBruto, hashToken, expiraEm);
    }
}
