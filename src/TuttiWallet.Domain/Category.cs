namespace TuttiWallet.Domain;

public class Category
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; }
    public TransactionType Type { get; }
    public Guid? ParentCategoryId { get; }

    public Category(Guid id, Guid userId, string name, TransactionType type, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome da categoria é obrigatório.", nameof(name));
        }

        if (parentCategoryId == id)
        {
            throw new ArgumentException("Uma categoria não pode ser subcategoria de si mesma.", nameof(parentCategoryId));
        }

        Id = id;
        UserId = userId;
        Name = name;
        Type = type;
        ParentCategoryId = parentCategoryId;
    }
}
