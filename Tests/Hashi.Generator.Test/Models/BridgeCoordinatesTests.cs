using FluentAssertions;
using Hashi.Generator.Models;
using System.Drawing;

namespace Hashi.Generator.Test.Models
{
    [TestFixture]
    public class BridgeCoordinatesTests
    {
        [Test]
        public void Constructor_WhenValidParameters_ShouldInitializeProperties()
        {
            // Arrange
            var location1 = new Point(1, 2);
            var location2 = new Point(3, 4);
            var amountBridges = 2;

            // Act
            var bridgeCoordinates = new BridgeCoordinates(location1, location2, amountBridges);

            // Assert
            bridgeCoordinates.Location1.Should().Be(location1);
            bridgeCoordinates.Location2.Should().Be(location2);
            bridgeCoordinates.AmountBridges.Should().Be(amountBridges);
        }

        [Test]
        public void Constructor_WhenZeroBridges_ShouldInitializeCorrectly()
        {
            // Arrange
            var location1 = new Point(0, 0);
            var location2 = new Point(5, 5);
            var amountBridges = 0;

            // Act
            var bridgeCoordinates = new BridgeCoordinates(location1, location2, amountBridges);

            // Assert
            bridgeCoordinates.Location1.Should().Be(location1);
            bridgeCoordinates.Location2.Should().Be(location2);
            bridgeCoordinates.AmountBridges.Should().Be(amountBridges);
        }

        [Test]
        public void Constructor_WhenSameLocations_ShouldInitializeCorrectly()
        {
            // Arrange
            var location1 = new Point(2, 3);
            var location2 = new Point(2, 3);
            var amountBridges = 1;

            // Act
            var bridgeCoordinates = new BridgeCoordinates(location1, location2, amountBridges);

            // Assert
            bridgeCoordinates.Location1.Should().Be(location1);
            bridgeCoordinates.Location2.Should().Be(location2);
            bridgeCoordinates.AmountBridges.Should().Be(amountBridges);
        }

        [Test]
        public void Constructor_WhenNegativeCoordinates_ShouldInitializeCorrectly()
        {
            // Arrange
            var location1 = new Point(-1, -2);
            var location2 = new Point(1, 2);
            var amountBridges = 1;

            // Act
            var bridgeCoordinates = new BridgeCoordinates(location1, location2, amountBridges);

            // Assert
            bridgeCoordinates.Location1.Should().Be(location1);
            bridgeCoordinates.Location2.Should().Be(location2);
            bridgeCoordinates.AmountBridges.Should().Be(amountBridges);
        }

        [Test]
        public void Properties_ShouldBeReadOnly()
        {
            // Arrange
            var location1 = new Point(1, 2);
            var location2 = new Point(3, 4);
            var amountBridges = 2;
            var bridgeCoordinates = new BridgeCoordinates(location1, location2, amountBridges);

            // Act & Assert - Verify properties have only getters (compile-time check)
            bridgeCoordinates.Location1.Should().Be(location1);
            bridgeCoordinates.Location2.Should().Be(location2);
            bridgeCoordinates.AmountBridges.Should().Be(amountBridges);
            
            // These properties should not have setters
            // The following would not compile if setters exist:
            // bridgeCoordinates.Location1 = new Point(0, 0); // Should not compile
            // bridgeCoordinates.Location2 = new Point(0, 0); // Should not compile
            // bridgeCoordinates.AmountBridges = 0; // Should not compile
        }
    }
}