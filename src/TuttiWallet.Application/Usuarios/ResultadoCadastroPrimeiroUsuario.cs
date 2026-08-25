namespace TuttiWallet.Application.Usuarios;

public sealed class ResultadoCadastroPrimeiroUsuario
{
    public StatusCadastroPrimeiroUsuario Status { get; }
    public Guid? UsuarioId { get; }
    public IReadOnlyDictionary<string, string[]> Erros { get; }

    private ResultadoCadastroPrimeiroUsuario(
        StatusCadastroPrimeiroUsuario status,
        Guid? usuarioId,
        IReadOnlyDictionary<string, string[]> erros)
    {
        Status = status;
        UsuarioId = usuarioId;
        Erros = erros;
    }

    public static ResultadoCadastroPrimeiroUsuario ComSucesso(Guid usuarioId) =>
        new(StatusCadastroPrimeiroUsuario.Sucesso, usuarioId, new Dictionary<string, string[]>());

    public static ResultadoCadastroPrimeiroUsuario ComUsuarioJaExistente() =>
        new(StatusCadastroPrimeiroUsuario.UsuarioJaExiste, null, new Dictionary<string, string[]>());

    public static ResultadoCadastroPrimeiroUsuario ComDadosInvalidos(Dictionary<string, List<string>> erros) =>
        new(
            StatusCadastroPrimeiroUsuario.DadosInvalidos,
            null,
            erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
}
