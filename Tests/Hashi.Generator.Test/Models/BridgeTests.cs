using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;

namespace Hashi.Generator.Test.Models
{
    [TestFixture]
    public class BridgeTests
    {
        private IIsland island1;
        private IIsland island2;

        [SetUp]
        public void Setup()
        {
            island1 = new Island(2, 1, 1);
            island2 = new Island(2, 1, 3);
        }

        [Test]
        public void Constructor_WhenValidParameters_ShouldInitializeProperties()
        {
            // Arrange
            int amountBridgesSet = 2;

            // Act
            var bridge = new Bridge(island1, island2, amountBridgesSet);

            // Assert
            bridge.Island1.Should().Be(island1);
            bridge.Island2.Should().Be(island2);
            bridge.AmountBridgesSet.Should().Be(amountBridgesSet);
        }

        [Test]
        public void AddOtherSide_WhenVerticalBridgeIsland1Above_ShouldUpdateBridgeCounts()
        {
            // Arrange
            var topIsland = new Island(2, 1, 1);
            var bottomIsland = new Island(2, 3, 1);
            var bridge = new Bridge(topIsland, bottomIsland, 1);

            // Act
            var otherSide = bridge.AddOtherSide();

            // Assert
            otherSide.Should().NotBeNull();
            otherSide.Island1.Should().Be(bottomIsland);
            otherSide.Island2.Should().Be(topIsland);
            topIsland.AmountBridgesDown.Should().Be(1);
            bottomIsland.AmountBridgesUp.Should().Be(1);
        }

        [Test]
        public void AddOtherSide_WhenVerticalBridgeIsland1Below_ShouldUpdateBridgeCounts()
        {
            // Arrange
            var bottomIsland = new Island(2, 3, 1);
            var topIsland = new Island(2, 1, 1);
            var bridge = new Bridge(bottomIsland, topIsland, 1);

            // Act
            var otherSide = bridge.AddOtherSide();

            // Assert
            otherSide.Should().NotBeNull();
            bottomIsland.AmountBridgesUp.Should().Be(1);
            topIsland.AmountBridgesDown.Should().Be(1);
        }

        [Test]
        public void AddOtherSide_WhenHorizontalBridgeIsland1Left_ShouldUpdateBridgeCounts()
        {
            // Arrange
            var leftIsland = new Island(2, 1, 1);
            var rightIsland = new Island(2, 1, 3);
            var bridge = new Bridge(leftIsland, rightIsland, 1);

            // Act
            var otherSide = bridge.AddOtherSide();

            // Assert
            otherSide.Should().NotBeNull();
            leftIsland.AmountBridgesRight.Should().Be(1);
            rightIsland.AmountBridgesLeft.Should().Be(1);
        }

        [Test]
        public void AddOtherSide_WhenHorizontalBridgeIsland1Right_ShouldUpdateBridgeCounts()
        {
            // Arrange
            var rightIsland = new Island(2, 1, 3);
            var leftIsland = new Island(2, 1, 1);
            var bridge = new Bridge(rightIsland, leftIsland, 1);

            // Act
            var otherSide = bridge.AddOtherSide();

            // Assert
            otherSide.Should().NotBeNull();
            rightIsland.AmountBridgesLeft.Should().Be(1);
            leftIsland.AmountBridgesRight.Should().Be(1);
        }

        [Test]
        public void AddOtherSide_WhenMultipleBridges_ShouldAddCorrectAmount()
        {
            // Arrange
            var leftIsland = new Island(4, 1, 1);
            var rightIsland = new Island(4, 1, 3);
            var bridge = new Bridge(leftIsland, rightIsland, 2);

            // Act
            var otherSide = bridge.AddOtherSide();

            // Assert
            leftIsland.AmountBridgesRight.Should().Be(2);
            rightIsland.AmountBridgesLeft.Should().Be(2);
            otherSide.AmountBridgesSet.Should().Be(2);
        }

        [Test]
        public void AddBridge_WhenIslandUpConnection_ShouldIncrementBridgeCountAndIslandValues()
        {
            // Arrange
            var bottomIsland = new Island(2, 2, 1);
            var topIsland = new Island(2, 1, 1);
            var islands = new List<IIsland> { bottomIsland, topIsland };
            
            var field = new int[][]
            {
                new int[] { 0, 0, 0 }, 
                new int[] { 0, 2, 0 }, // topIsland at (1,1)
                new int[] { 0, 2, 0 }  // bottomIsland at (2,1)
            };

            // Set up neighbors using SetAllNeighbors
            bottomIsland.SetAllNeighbors(field, islands);
            topIsland.SetAllNeighbors(field, islands);
            
            var bridge = new Bridge(bottomIsland, topIsland, 0);

            // Act
            bridge.AddBridge(field);

            // Assert
            bridge.AmountBridgesSet.Should().Be(1);
            bottomIsland.AmountBridgesUp.Should().Be(1);
            topIsland.AmountBridgesDown.Should().Be(1);
            bottomIsland.AmountBridgesConnectable.Should().Be(3); // 2 + 1
            topIsland.AmountBridgesConnectable.Should().Be(3); // 2 + 1
            field[2][1].Should().Be(3); // bottomIsland field value incremented
            field[1][1].Should().Be(3); // topIsland field value incremented
        }

        [Test]
        public void AddBridge_WhenIslandDownConnection_ShouldIncrementBridgeCountAndIslandValues()
        {
            // Arrange
            var topIsland = new Island(2, 1, 1);
            var bottomIsland = new Island(2, 2, 1);
            var islands = new List<IIsland> { topIsland, bottomIsland };
            
            var field = new int[][]
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 2, 0 }, // topIsland at (1,1)
                new int[] { 0, 2, 0 }  // bottomIsland at (2,1)
            };

            // Set up neighbors using SetAllNeighbors
            topIsland.SetAllNeighbors(field, islands);
            bottomIsland.SetAllNeighbors(field, islands);
            
            var bridge = new Bridge(topIsland, bottomIsland, 0);

            // Act
            bridge.AddBridge(field);

            // Assert
            bridge.AmountBridgesSet.Should().Be(1);
            topIsland.AmountBridgesDown.Should().Be(1);
            bottomIsland.AmountBridgesUp.Should().Be(1);
        }

        [Test]
        public void AddBridge_WhenIslandLeftConnection_ShouldIncrementBridgeCountAndIslandValues()
        {
            // Arrange
            var rightIsland = new Island(2, 1, 2);
            var leftIsland = new Island(2, 1, 1);
            var islands = new List<IIsland> { rightIsland, leftIsland };
            
            var field = new int[][]
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 2, 2 }, // leftIsland at (1,1), rightIsland at (1,2)
                new int[] { 0, 0, 0 }
            };

            // Set up neighbors using SetAllNeighbors
            rightIsland.SetAllNeighbors(field, islands);
            leftIsland.SetAllNeighbors(field, islands);
            
            var bridge = new Bridge(rightIsland, leftIsland, 0);

            // Act
            bridge.AddBridge(field);

            // Assert
            bridge.AmountBridgesSet.Should().Be(1);
            rightIsland.AmountBridgesLeft.Should().Be(1);
            leftIsland.AmountBridgesRight.Should().Be(1);
        }

        [Test]
        public void AddBridge_WhenIslandRightConnection_ShouldIncrementBridgeCountAndIslandValues()
        {
            // Arrange
            var leftIsland = new Island(2, 1, 1);
            var rightIsland = new Island(2, 1, 2);
            var islands = new List<IIsland> { leftIsland, rightIsland };
            
            var field = new int[][]
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 2, 2 }, // leftIsland at (1,1), rightIsland at (1,2)
                new int[] { 0, 0, 0 }
            };

            // Set up neighbors using SetAllNeighbors
            leftIsland.SetAllNeighbors(field, islands);
            rightIsland.SetAllNeighbors(field, islands);
            
            var bridge = new Bridge(leftIsland, rightIsland, 0);

            // Act
            bridge.AddBridge(field);

            // Assert
            bridge.AmountBridgesSet.Should().Be(1);
            leftIsland.AmountBridgesRight.Should().Be(1);
            rightIsland.AmountBridgesLeft.Should().Be(1);
        }

        [Test]
        public void AddBridge_WhenCalledMultipleTimes_ShouldIncrementCorrectly()
        {
            // Arrange
            var leftIsland = new Island(4, 1, 1);
            var rightIsland = new Island(4, 1, 2);
            var islands = new List<IIsland> { leftIsland, rightIsland };
            
            var field = new int[][]
            {
                new int[] { 0, 0, 0 },
                new int[] { 0, 4, 4 }, // leftIsland at (1,1), rightIsland at (1,2)
                new int[] { 0, 0, 0 }
            };

            // Set up neighbors using SetAllNeighbors
            leftIsland.SetAllNeighbors(field, islands);
            rightIsland.SetAllNeighbors(field, islands);
            
            var bridge = new Bridge(leftIsland, rightIsland, 0);

            // Act
            bridge.AddBridge(field);
            bridge.AddBridge(field);

            // Assert
            bridge.AmountBridgesSet.Should().Be(2);
            leftIsland.AmountBridgesRight.Should().Be(2);
            rightIsland.AmountBridgesLeft.Should().Be(2);
            leftIsland.AmountBridgesConnectable.Should().Be(6); // 4 + 2
            rightIsland.AmountBridgesConnectable.Should().Be(6); // 4 + 2
            field[1][1].Should().Be(6); // leftIsland field value
            field[1][2].Should().Be(6); // rightIsland field value
        }

        [Test]
        public void AddBridge_WhenNoDirectConnection_ShouldOnlyIncrementBridgeCount()
        {
            // Arrange - Islands that are not connected as neighbors
            var island1 = new Island(2, 1, 1);
            var island2 = new Island(2, 3, 3);
            
            var bridge = new Bridge(island1, island2, 0);
            var field = new int[][]
            {
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 2, 0, 0 }, // island1 at (1,1)
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 2 }  // island2 at (3,3)
            };

            // Act
            bridge.AddBridge(field);

            // Assert
            bridge.AmountBridgesSet.Should().Be(1);
            island1.AmountBridgesConnectable.Should().Be(3); // 2 + 1
            island2.AmountBridgesConnectable.Should().Be(3); // 2 + 1
            field[1][1].Should().Be(3); // island1 field value incremented
            field[3][3].Should().Be(3); // island2 field value incremented
            
            // Direction specific bridge counts should remain 0 since they're not connected
            island1.AmountBridgesUp.Should().Be(0);
            island1.AmountBridgesDown.Should().Be(0);
            island1.AmountBridgesLeft.Should().Be(0);
            island1.AmountBridgesRight.Should().Be(0);
        }
    }
}