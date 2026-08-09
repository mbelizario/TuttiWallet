using FluentAssertions;
using TuttiWallet.Domain;

namespace TuttiWallet.Domain.Tests;

public class TransactionTests
{
    [Fact]
    public void Constructor_WithValidAmount_CreatesTransaction()
    {
        var transaction = new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            TransactionType.Expense,
            amount: 150.75m,
            occurredOn: new DateOnly(2026, 8, 8));

        transaction.Amount.Should().Be(150.75m);
        transaction.Type.Should().Be(TransactionType.Expense);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithNonPositiveAmount_Throws(decimal amount)
    {
        var act = () => new Transaction(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            TransactionType.Income,
            amount,
            new DateOnly(2026, 8, 8));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
