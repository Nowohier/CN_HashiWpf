using System.Collections.ObjectModel;
using FluentAssertions;
using Hashi.Gui.Extensions;

namespace Hashi.Gui.Test.Extensions;

[TestFixture]
public class ObservableCollectionExtensionsTests
{
    #region AddRange Tests

    [Test]
    public void AddRange_WhenCalledWithItems_ShouldAddAllItems()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2 };
        var items = new List<int> { 3, 4, 5 };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5 });
    }

    [Test]
    public void AddRange_WhenCalledWithEmptyItems_ShouldNotModifyCollection()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2 };
        var items = new List<int>();

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 2 });
    }

    [Test]
    public void AddRange_WhenCalledOnEmptyCollection_ShouldAddAllItems()
    {
        // Arrange
        var collection = new ObservableCollection<string>();
        var items = new List<string> { "a", "b", "c" };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().BeEquivalentTo(new[] { "a", "b", "c" });
    }

    [Test]
    public void AddRange_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ObservableCollection<int>? collection = null;
        var items = new List<int> { 1, 2 };

        // Act & Assert
        var action = () => collection!.AddRange(items);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("collection");
    }

    [Test]
    public void AddRange_WhenItemsIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var collection = new ObservableCollection<int>();
        IEnumerable<int>? items = null;

        // Act & Assert
        var action = () => collection.AddRange(items!);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("items");
    }

    [Test]
    public void AddRange_WhenCalledWithSingleItem_ShouldAddItem()
    {
        // Arrange
        var collection = new ObservableCollection<int>();
        var items = new List<int> { 42 };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().ContainSingle().Which.Should().Be(42);
    }

    #endregion

    #region RemoveAll Tests

    [Test]
    public void RemoveAll_WhenPredicateMatches_ShouldRemoveMatchingItems()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2, 3, 4, 5 };

        // Act
        collection.RemoveAll(x => x > 3);

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Test]
    public void RemoveAll_WhenPredicateMatchesNothing_ShouldNotModifyCollection()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2, 3 };

        // Act
        collection.RemoveAll(x => x > 10);

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Test]
    public void RemoveAll_WhenPredicateMatchesAll_ShouldRemoveAllItems()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2, 3 };

        // Act
        collection.RemoveAll(_ => true);

        // Assert
        collection.Should().BeEmpty();
    }

    [Test]
    public void RemoveAll_WhenCollectionIsEmpty_ShouldNotThrow()
    {
        // Arrange
        var collection = new ObservableCollection<int>();

        // Act & Assert
        var action = () => collection.RemoveAll(x => x > 0);
        action.Should().NotThrow();
        collection.Should().BeEmpty();
    }

    [Test]
    public void RemoveAll_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ObservableCollection<int>? collection = null;

        // Act & Assert
        var action = () => collection!.RemoveAll(x => x > 0);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("collection");
    }

    [Test]
    public void RemoveAll_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2, 3 };
        Func<int, bool>? predicate = null;

        // Act & Assert
        var action = () => collection.RemoveAll(predicate!);
        action.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("predicate");
    }

    [Test]
    public void RemoveAll_WhenRemovingEvenNumbers_ShouldKeepOddNumbers()
    {
        // Arrange
        var collection = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6 };

        // Act
        collection.RemoveAll(x => x % 2 == 0);

        // Assert
        collection.Should().BeEquivalentTo(new[] { 1, 3, 5 });
    }

    [Test]
    public void RemoveAll_WhenRemovingStrings_ShouldWorkWithStringPredicate()
    {
        // Arrange
        var collection = new ObservableCollection<string> { "apple", "banana", "cherry", "avocado" };

        // Act
        collection.RemoveAll(s => s.StartsWith('a'));

        // Assert
        collection.Should().BeEquivalentTo(new[] { "banana", "cherry" });
    }

    #endregion
}
