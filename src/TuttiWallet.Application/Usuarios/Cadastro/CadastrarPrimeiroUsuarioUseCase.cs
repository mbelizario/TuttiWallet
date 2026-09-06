using TuttiWallet.Domain;

namespace TuttiWallet.Application.Usuarios.Cadastro;

public sealed class CadastrarPrimeiroUsuarioUseCase(IUsuarioRepository usuarioRepository, ISenhaHasher senhaHasher)
{
    public async Task<ResultadoCadastroPrimeiroUsuario> ExecutarAsync(CadastrarPrimeiroUsuarioComando comando)
    {
        if (await usuarioRepository.ObterQuantidadeUsuariosAsync() > 0)
            return ResultadoCadastroPrimeiroUsuario.ComUsuarioJaExistente();

        var erros = CadastroUsuarioValidador.Validar(comando);

        if (await usuarioRepository.ObterExistePorEmailAsync(comando.Email))
            CadastroUsuarioValidador.AdicionarErro(
                erros,
                nameof(CadastrarPrimeiroUsuarioComando.Email),
                "Este e-mail já está cadastrado.");

        if (erros.Count > 0)
            return ResultadoCadastroPrimeiroUsuario.ComDadosInvalidos(erros);

        var celularSomenteDigitos = new string(comando.Celular.Where(char.IsDigit).ToArray());
        var hashSenha = senhaHasher.GerarHash(comando.Senha);

        var usuario = new Usuario(
            Guid.NewGuid(),
            comando.Nome,
            comando.Sobrenome,
            comando.Email,
            celularSomenteDigitos,
            hashSenha);

        await usuarioRepository.InserirAsync(usuario);

        return ResultadoCadastroPrimeiroUsuario.ComSucesso(usuario.Id);
    }
}
