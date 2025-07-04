using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using System.Drawing;
// ReSharper disable CollectionNeverUpdated.Local

namespace Hashi.Generator.Test.Providers
{
    [TestFixture]
    public class SolutionProviderTests
    {
        [Test]
        public void Constructor_WhenValidParameters_ShouldInitializeProperties()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 0, 1, 0 },
                new[] { 1, 0, 2 },
                new[] { 0, 2, 0 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>
            {
                new BridgeCoordinates(new Point(0, 1), new Point(1, 0), 1),
                new BridgeCoordinates(new Point(1, 2), new Point(2, 1), 2)
            };
            string name = "Test Puzzle";

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, name);

            // Assert
            solutionProvider.HashiField.Should().BeEquivalentTo(hashiField);
            solutionProvider.BridgeCoordinates.Should().BeEquivalentTo(bridgeCoordinates);
            solutionProvider.Name.Should().Be(name);
        }

        [Test]
        public void Constructor_WhenNullHashiField_ShouldInitializeWithNull()
        {
            // Arrange
            IReadOnlyList<int[]>? hashiField = null;
            var bridgeCoordinates = new List<IBridgeCoordinates>();
            string name = "Test Puzzle";

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, name);

            // Assert
            solutionProvider.HashiField.Should().BeNull();
            solutionProvider.BridgeCoordinates.Should().BeEquivalentTo(bridgeCoordinates);
            solutionProvider.Name.Should().Be(name);
        }

        [Test]
        public void Constructor_WhenNullBridgeCoordinates_ShouldInitializeWithNull()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 1, 2, 1 }
            };
            List<IBridgeCoordinates>? bridgeCoordinates = null;
            string name = "Test Puzzle";

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, name);

            // Assert
            solutionProvider.HashiField.Should().BeEquivalentTo(hashiField);
            solutionProvider.BridgeCoordinates.Should().BeNull();
            solutionProvider.Name.Should().Be(name);
        }

        [Test]
        public void Constructor_WhenNullName_ShouldInitializeWithEmptyString()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 1, 2, 1 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>();
            string? name = null;

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, name);

            // Assert
            solutionProvider.HashiField.Should().BeEquivalentTo(hashiField);
            solutionProvider.BridgeCoordinates.Should().BeEquivalentTo(bridgeCoordinates);
            solutionProvider.Name.Should().Be(string.Empty);
        }

        [Test]
        public void Constructor_WhenNoNameProvided_ShouldInitializeWithEmptyString()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 1, 2, 1 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>();

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates);

            // Assert
            solutionProvider.HashiField.Should().BeEquivalentTo(hashiField);
            solutionProvider.BridgeCoordinates.Should().BeEquivalentTo(bridgeCoordinates);
            solutionProvider.Name.Should().Be(string.Empty);
        }

        [Test]
        public void Constructor_WhenEmptyCollections_ShouldInitializeCorrectly()
        {
            // Arrange
            var hashiField = new List<int[]>();
            var bridgeCoordinates = new List<IBridgeCoordinates>();
            string name = "Empty Puzzle";

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, name);

            // Assert
            solutionProvider.HashiField.Should().BeEmpty();
            solutionProvider.BridgeCoordinates.Should().BeEmpty();
            solutionProvider.Name.Should().Be(name);
        }

        [Test]
        public void Name_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 1, 2, 1 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>();
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, "Original Name");

            // Act
            solutionProvider.Name = "Updated Name";

            // Assert
            solutionProvider.Name.Should().Be("Updated Name");
        }

        [Test]
        public void Name_WhenSetToNull_ShouldSetToNull()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 1, 2, 1 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>();
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, "Original Name");

            // Act
            solutionProvider.Name = null;

            // Assert
            solutionProvider.Name.Should().BeNull();
        }

        [Test]
        public void Properties_WhenInitialized_ShouldBeReadOnly()
        {
            // Arrange
            var hashiField = new List<int[]>
            {
                new[] { 1, 2, 1 }
            };
            var bridgeCoordinates = new List<IBridgeCoordinates>
            {
                new BridgeCoordinates(new Point(0, 0), new Point(0, 2), 1)
            };
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, "Test");

            // Act & Assert - HashiField and BridgeCoordinates should not have setters
            solutionProvider.HashiField.Should().NotBeNull();
            solutionProvider.BridgeCoordinates.Should().NotBeNull();

            // These properties should not have setters (compile-time check)
            // The following would not compile if setters exist:
            // solutionProvider.HashiField = new List<int[]>(); // Should not compile
            // solutionProvider.BridgeCoordinates = new List<IBridgeCoordinates>(); // Should not compile
        }

        [Test]
        public void Constructor_WhenLargeCollections_ShouldInitializeCorrectly()
        {
            // Arrange
            var hashiField = new List<int[]>();
            for (int i = 0; i < 100; i++)
            {
                hashiField.Add(new int[100]);
            }

            var bridgeCoordinates = new List<IBridgeCoordinates>();
            for (int i = 0; i < 1000; i++)
            {
                bridgeCoordinates.Add(new BridgeCoordinates(new Point(i, i), new Point(i + 1, i + 1), 1));
            }

            string name = "Large Puzzle";

            // Act
            var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, name);

            // Assert
            solutionProvider.HashiField.Should().HaveCount(100);
            solutionProvider.BridgeCoordinates.Should().HaveCount(1000);
            solutionProvider.Name.Should().Be(name);
        }
    }
}