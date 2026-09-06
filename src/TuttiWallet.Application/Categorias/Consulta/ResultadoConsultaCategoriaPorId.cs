using TuttiWallet.Domain;

namespace TuttiWallet.Application.Categorias.Consulta;

public sealed class ResultadoConsultaCategoriaPorId
{
    public StatusConsultaCategoriaPorId Status { get; }
    public Categoria? Categoria { get; }
    public IReadOnlyList<Categoria> Subcategorias { get; }

    private ResultadoConsultaCategoriaPorId(
        StatusConsultaCategoriaPorId status,
        Categoria? categoria,
        IReadOnlyList<Categoria> subcategorias)
    {
        Status = status;
        Categoria = categoria;
        Subcategorias = subcategorias;
    }

    public static ResultadoConsultaCategoriaPorId ComSucesso(Categoria categoria, IReadOnlyList<Categoria> subcategorias) =>
        new(StatusConsultaCategoriaPorId.Sucesso, categoria, subcategorias);

    public static ResultadoConsultaCategoriaPorId ComCategoriaNaoEncontrada() =>
        new(StatusConsultaCategoriaPorId.NaoEncontrada, null, []);
}
