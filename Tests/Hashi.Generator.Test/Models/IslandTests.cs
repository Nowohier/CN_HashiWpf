using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;

namespace Hashi.Generator.Test.Models
{
    [TestFixture]
    public class IslandTests
    {
        [Test]
        public void Constructor_WhenValidParameters_ShouldInitializeProperties()
        {
            // Arrange
            var amountBridgesConnectable = 3;
            var y = 5;
            var x = 7;

            // Act
            var island = new Island(amountBridgesConnectable, y, x);

            // Assert
            island.AmountBridgesConnectable.Should().Be(amountBridgesConnectable);
            island.Y.Should().Be(y);
            island.X.Should().Be(x);
            island.AmountBridgesUp.Should().Be(0);
            island.AmountBridgesDown.Should().Be(0);
            island.AmountBridgesLeft.Should().Be(0);
            island.AmountBridgesRight.Should().Be(0);
            island.IslandUp.Should().BeNull();
            island.IslandDown.Should().BeNull();
            island.IslandLeft.Should().BeNull();
            island.IslandRight.Should().BeNull();
        }

        [Test]
        public void SetAllNeighbors_WhenIslandAbove_ShouldSetIslandUp()
        {
            // Arrange
            var mainIsland = new Island(2, 2, 1);
            var upIsland = new Island(1, 1, 1);
            var islands = new List<IIsland> { mainIsland, upIsland };
            var field = new int[][]
            {
                [0, 0, 0],
                [0, 1, 0], // upIsland at (1,1)
                [0, 2, 0], // mainIsland at (2,1)
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandUp.Should().Be(upIsland);
            mainIsland.IslandDown.Should().BeNull();
            mainIsland.IslandLeft.Should().BeNull();
            mainIsland.IslandRight.Should().BeNull();
        }

        [Test]
        public void SetAllNeighbors_WhenIslandBelow_ShouldSetIslandDown()
        {
            // Arrange
            var mainIsland = new Island(2, 1, 1);
            var downIsland = new Island(1, 2, 1);
            var islands = new List<IIsland> { mainIsland, downIsland };
            var field = new int[][]
            {
                [0, 0, 0],
                [0, 2, 0], // mainIsland at (1,1)
                [0, 1, 0], // downIsland at (2,1)
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandDown.Should().Be(downIsland);
            mainIsland.IslandUp.Should().BeNull();
            mainIsland.IslandLeft.Should().BeNull();
            mainIsland.IslandRight.Should().BeNull();
        }

        [Test]
        public void SetAllNeighbors_WhenIslandLeft_ShouldSetIslandLeft()
        {
            // Arrange
            var mainIsland = new Island(2, 1, 2);
            var leftIsland = new Island(1, 1, 1);
            var islands = new List<IIsland> { mainIsland, leftIsland };
            var field = new int[][]
            {
                [0, 0, 0],
                [0, 1, 2], // leftIsland at (1,1), mainIsland at (1,2)
                [0, 0, 0],
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandLeft.Should().Be(leftIsland);
            mainIsland.IslandUp.Should().BeNull();
            mainIsland.IslandDown.Should().BeNull();
            mainIsland.IslandRight.Should().BeNull();
        }

        [Test]
        public void SetAllNeighbors_WhenIslandRight_ShouldSetIslandRight()
        {
            // Arrange
            var mainIsland = new Island(2, 1, 1);
            var rightIsland = new Island(1, 1, 2);
            var islands = new List<IIsland> { mainIsland, rightIsland };
            var field = new int[][]
            {
                [0, 0, 0],
                [0, 2, 1], // mainIsland at (1,1), rightIsland at (1,2)
                [0, 0, 0],
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandRight.Should().Be(rightIsland);
            mainIsland.IslandUp.Should().BeNull();
            mainIsland.IslandDown.Should().BeNull();
            mainIsland.IslandLeft.Should().BeNull();
        }

        [Test]
        public void SetAllNeighbors_WhenMultipleNeighbors_ShouldSetAllDirections()
        {
            // Arrange
            var mainIsland = new Island(4, 2, 2);
            var upIsland = new Island(1, 1, 2);
            var downIsland = new Island(1, 3, 2);
            var leftIsland = new Island(1, 2, 1);
            var rightIsland = new Island(1, 2, 3);
            var islands = new List<IIsland> { mainIsland, upIsland, downIsland, leftIsland, rightIsland };
            var field = new int[][]
            {
                [0, 0, 0, 0, 0],
                [0, 0, 1, 0, 0], // upIsland at (1,2)
                [0, 1, 4, 1, 0], // leftIsland at (2,1), mainIsland at (2,2), rightIsland at (2,3)
                [0, 0, 1, 0, 0], // downIsland at (3,2)
                [0, 0, 0, 0, 0],
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandUp.Should().Be(upIsland);
            mainIsland.IslandDown.Should().Be(downIsland);
            mainIsland.IslandLeft.Should().Be(leftIsland);
            mainIsland.IslandRight.Should().Be(rightIsland);
        }

        [Test]
        public void SetAllNeighbors_WhenNoNeighbors_ShouldLeaveAllNeighborsNull()
        {
            // Arrange
            var mainIsland = new Island(1, 2, 2);
            var islands = new List<IIsland> { mainIsland };
            var field = new int[][]
            {
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
                [0, 0, 1, 0, 0], // mainIsland at (2,2) - isolated
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandUp.Should().BeNull();
            mainIsland.IslandDown.Should().BeNull();
            mainIsland.IslandLeft.Should().BeNull();
            mainIsland.IslandRight.Should().BeNull();
        }

        [Test]
        public void SetAllNeighbors_WhenObstaclesBetweenIslands_ShouldStillSetNeighbors()
        {
            // Arrange
            var mainIsland = new Island(2, 2, 2);
            var potentialNeighbor = new Island(1, 2, 4);
            var islands = new List<IIsland> { mainIsland, potentialNeighbor };
            var field = new int[][]
            {
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
                [0, 0, 2, 0, 1], // mainIsland at (2,2), gap at (2,3), neighbor at (2,4)
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
            };

            // Act
            mainIsland.SetAllNeighbors(field, islands);

            // Assert
            mainIsland.IslandRight.Should().Be(potentialNeighbor); // Should find the neighbor despite gap
        }

        [Test]
        public void AmountBridgesConnectable_WhenSet_ShouldUpdateValue()
        {
            // Arrange
            var island = new Island(2, 1, 1);

            // Act
            island.AmountBridgesConnectable = 5;

            // Assert
            island.AmountBridgesConnectable.Should().Be(5);
        }

        [Test]
        public void BridgeDirections_WhenSet_ShouldUpdateValues()
        {
            // Arrange
            var island = new Island(2, 1, 1);

            // Act
            island.AmountBridgesUp = 1;
            island.AmountBridgesDown = 2;
            island.AmountBridgesLeft = 1;
            island.AmountBridgesRight = 2;

            // Assert
            island.AmountBridgesUp.Should().Be(1);
            island.AmountBridgesDown.Should().Be(2);
            island.AmountBridgesLeft.Should().Be(1);
            island.AmountBridgesRight.Should().Be(2);
        }
    }
}