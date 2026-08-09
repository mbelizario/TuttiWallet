using FluentAssertions;
using TuttiWallet.Domain;

namespace TuttiWallet.Domain.Tests;

public class CategoryTests
{
    [Fact]
    public void Constructor_WithParentCategory_CreatesSubcategory()
    {
        var parentId = Guid.NewGuid();

        var subcategory = new Category(Guid.NewGuid(), Guid.NewGuid(), "Streaming", TransactionType.Expense, parentId);

        subcategory.ParentCategoryId.Should().Be(parentId);
    }

    [Fact]
    public void Constructor_AsOwnParent_Throws()
    {
        var id = Guid.NewGuid();

        var act = () => new Category(id, Guid.NewGuid(), "Lazer", TransactionType.Expense, id);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithoutName_Throws()
    {
        var act = () => new Category(Guid.NewGuid(), Guid.NewGuid(), "  ", TransactionType.Income);

        act.Should().Throw<ArgumentException>();
    }
}
