using TuttiWallet.Domain;

namespace TuttiWallet.Application.Usuarios;

public interface IUsuarioRepository
{
    Task<int> ObterQuantidadeUsuariosAsync();

    Task<bool> ObterExistePorEmailAsync(string email);

    Task InserirAsync(Usuario usuario);
}
