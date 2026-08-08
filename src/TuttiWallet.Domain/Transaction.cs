namespace TuttiWallet.Domain;

public class Transaction
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid CategoryId { get; }
    public TransactionType Type { get; }
    public decimal Amount { get; }
    public DateOnly OccurredOn { get; }
    public string? Description { get; }

    public Transaction(
        Guid id,
        Guid userId,
        Guid categoryId,
        TransactionType type,
        decimal amount,
        DateOnly occurredOn,
        string? description = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "O valor da transação deve ser maior que zero.");
        }

        Id = id;
        UserId = userId;
        CategoryId = categoryId;
        Type = type;
        Amount = amount;
        OccurredOn = occurredOn;
        Description = description;
    }
}
