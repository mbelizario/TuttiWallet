namespace TuttiWallet.Domain;

public class Categoria
{
    public Guid Id { get; }
    public Guid UsuarioId { get; }
    public string Nome { get; }
    public TipoTransacao Tipo { get; }
    public Guid? CategoriaPaiId { get; }

    public Categoria(Guid id, Guid usuarioId, string nome, TipoTransacao tipo, Guid? categoriaPaiId = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome da categoria é obrigatório.", nameof(nome));
        }

        if (categoriaPaiId == id)
        {
            throw new ArgumentException("Uma categoria não pode ser subcategoria de si mesma.", nameof(categoriaPaiId));
        }

        Id = id;
        UsuarioId = usuarioId;
        Nome = nome;
        Tipo = tipo;
        CategoriaPaiId = categoriaPaiId;
    }
}
