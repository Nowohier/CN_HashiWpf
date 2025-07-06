using FluentAssertions;
using Hashi.Gui.Extensions;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Test.Extensions;

[TestFixture]
public class ObservableCollectionExtensionsTests
{
    private ObservableCollection<string> collection;

    [SetUp]
    public void SetUp()
    {
        collection = new ObservableCollection<string>();
    }

    [Test]
    public void AddRange_WhenCalledWithValidItems_ShouldAddAllItems()
    {
        // Arrange
        var items = new[] { "Item1", "Item2", "Item3" };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder("Item1", "Item2", "Item3");
    }

    [Test]
    public void AddRange_WhenCalledWithEmptyCollection_ShouldNotAddAnyItems()
    {
        // Arrange
        var items = new string[0];

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().BeEmpty();
    }

    [Test]
    public void AddRange_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ObservableCollection<string>? nullCollection = null;
        var items = new[] { "Item1" };

        // Act & Assert
        var act = () => nullCollection!.AddRange(items);
        act.Should().Throw<ArgumentNullException>().WithParameterName("collection");
    }

    [Test]
    public void AddRange_WhenItemsIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => collection.AddRange(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("items");
    }

    [Test]
    public void AddRange_WhenCollectionAlreadyHasItems_ShouldAppendNewItems()
    {
        // Arrange
        collection.Add("ExistingItem");
        var items = new[] { "Item1", "Item2" };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder("ExistingItem", "Item1", "Item2");
    }

    [Test]
    public void AddRange_WhenCalledWithSingleItem_ShouldAddThatItem()
    {
        // Arrange
        var items = new[] { "SingleItem" };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().HaveCount(1);
        collection.Should().Contain("SingleItem");
    }

    [Test]
    public void AddRange_WhenCalledWithDuplicateItems_ShouldAddAllDuplicates()
    {
        // Arrange
        var items = new[] { "Item1", "Item1", "Item2" };

        // Act
        collection.AddRange(items);

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder("Item1", "Item1", "Item2");
        collection.Count(x => x == "Item1").Should().Be(2);
    }

    [Test]
    public void AddRange_WhenCalledWithNullItems_ShouldAddNullItems()
    {
        // Arrange
        var items = new string?[] { "Item1", null, "Item2" };

        // Act
        collection.AddRange(items!);

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder("Item1", null, "Item2");
    }

    [Test]
    public void RemoveAll_WhenCalledWithValidPredicate_ShouldRemoveMatchingItems()
    {
        // Arrange
        collection.AddRange(new[] { "Apple", "Banana", "Cherry", "Date" });

        // Act
        collection.RemoveAll(x => x.StartsWith("A") || x.StartsWith("C"));

        // Assert
        collection.Should().HaveCount(2);
        collection.Should().ContainInOrder("Banana", "Date");
    }

    [Test]
    public void RemoveAll_WhenNoItemsMatch_ShouldNotRemoveAnyItems()
    {
        // Arrange
        collection.AddRange(new[] { "Apple", "Banana", "Cherry" });

        // Act
        collection.RemoveAll(x => x.StartsWith("Z"));

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder("Apple", "Banana", "Cherry");
    }

    [Test]
    public void RemoveAll_WhenAllItemsMatch_ShouldRemoveAllItems()
    {
        // Arrange
        collection.AddRange(new[] { "Apple", "Apricot", "Avocado" });

        // Act
        collection.RemoveAll(x => x.StartsWith("A"));

        // Assert
        collection.Should().BeEmpty();
    }

    [Test]
    public void RemoveAll_WhenCollectionIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        ObservableCollection<string>? nullCollection = null;

        // Act & Assert
        var act = () => nullCollection!.RemoveAll(x => true);
        act.Should().Throw<ArgumentNullException>().WithParameterName("collection");
    }

    [Test]
    public void RemoveAll_WhenPredicateIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => collection.RemoveAll(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("predicate");
    }

    [Test]
    public void RemoveAll_WhenCollectionIsEmpty_ShouldNotThrow()
    {
        // Act & Assert
        var act = () => collection.RemoveAll(x => true);
        act.Should().NotThrow();
        collection.Should().BeEmpty();
    }

    [Test]
    public void RemoveAll_WhenCalledMultipleTimes_ShouldWorkCorrectly()
    {
        // Arrange
        collection.AddRange(new[] { "A1", "B1", "A2", "B2", "A3" });

        // Act
        collection.RemoveAll(x => x.StartsWith("A"));
        collection.RemoveAll(x => x.Contains("1"));

        // Assert
        collection.Should().HaveCount(1);
        collection.Should().Contain("B2");
    }

    [Test]
    public void AddRange_WithDifferentDataTypes_ShouldWork()
    {
        // Arrange
        var intCollection = new ObservableCollection<int>();
        var items = new[] { 1, 2, 3, 4, 5 };

        // Act
        intCollection.AddRange(items);

        // Assert
        intCollection.Should().HaveCount(5);
        intCollection.Should().ContainInOrder(1, 2, 3, 4, 5);
    }

    [Test]
    public void RemoveAll_WithDifferentDataTypes_ShouldWork()
    {
        // Arrange
        var intCollection = new ObservableCollection<int>();
        intCollection.AddRange(new[] { 1, 2, 3, 4, 5, 6 });

        // Act
        intCollection.RemoveAll(x => x % 2 == 0); // Remove even numbers

        // Assert
        intCollection.Should().HaveCount(3);
        intCollection.Should().ContainInOrder(1, 3, 5);
    }

    [Test]
    public void AddRange_WithComplexObjects_ShouldWork()
    {
        // Arrange
        var complexCollection = new ObservableCollection<TestObject>();
        var items = new[]
        {
            new TestObject { Id = 1, Name = "Object1" },
            new TestObject { Id = 2, Name = "Object2" }
        };

        // Act
        complexCollection.AddRange(items);

        // Assert
        complexCollection.Should().HaveCount(2);
        complexCollection[0].Id.Should().Be(1);
        complexCollection[1].Id.Should().Be(2);
    }

    [Test]
    public void RemoveAll_WithComplexObjects_ShouldWork()
    {
        // Arrange
        var complexCollection = new ObservableCollection<TestObject>();
        var items = new[]
        {
            new TestObject { Id = 1, Name = "Keep" },
            new TestObject { Id = 2, Name = "Remove" },
            new TestObject { Id = 3, Name = "Keep" }
        };
        complexCollection.AddRange(items);

        // Act
        complexCollection.RemoveAll(x => x.Name == "Remove");

        // Assert
        complexCollection.Should().HaveCount(2);
        complexCollection.Should().AllSatisfy(x => x.Name.Should().Be("Keep"));
    }

    [Test]
    public void Extensions_ShouldBeStaticMethods()
    {
        // Arrange
        var type = typeof(ObservableCollectionExtensions);

        // Act
        var addRangeMethod = type.GetMethod(nameof(ObservableCollectionExtensions.AddRange));
        var removeAllMethod = type.GetMethod(nameof(ObservableCollectionExtensions.RemoveAll));

        // Assert
        addRangeMethod.Should().NotBeNull();
        addRangeMethod!.IsStatic.Should().BeTrue();
        addRangeMethod.IsPublic.Should().BeTrue();

        removeAllMethod.Should().NotBeNull();
        removeAllMethod!.IsStatic.Should().BeTrue();
        removeAllMethod.IsPublic.Should().BeTrue();
    }

    [Test]
    public void AddRange_ExtensionMethod_ShouldBeAccessibleOnObservableCollection()
    {
        // Arrange
        var testCollection = new ObservableCollection<int>();
        var items = new[] { 1, 2, 3 };

        // Act & Assert
        // If this compiles and runs, the extension method is properly accessible
        testCollection.AddRange(items);
        testCollection.Should().HaveCount(3);
    }

    [Test]
    public void RemoveAll_ExtensionMethod_ShouldBeAccessibleOnObservableCollection()
    {
        // Arrange
        var testCollection = new ObservableCollection<int>();
        testCollection.AddRange(new[] { 1, 2, 3, 4 });

        // Act & Assert
        // If this compiles and runs, the extension method is properly accessible
        testCollection.RemoveAll(x => x > 2);
        testCollection.Should().HaveCount(2);
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}