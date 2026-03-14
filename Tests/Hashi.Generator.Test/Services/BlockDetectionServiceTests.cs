using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Services;
using Moq;

namespace Hashi.Generator.Test.Services;

[TestFixture]
public class BlockDetectionServiceTests
{
    private BlockDetectionService blockDetectionService = null!;

    [SetUp]
    public void Setup()
    {
        blockDetectionService = new BlockDetectionService();
    }

    #region GetDownBlockedBetween Tests

    [Test]
    public void GetDownBlockedBetween_WhenIslandDownIsNull_ShouldReturnNegativeOne()
    {
        // Arrange
        var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
        mockIsland.Setup(i => i.IslandDown).Returns((IIsland?)null);
        var field = new int[][] { [0, 0, 0] };

        // Act
        var result = blockDetectionService.GetDownBlockedBetween(mockIsland.Object, field, new List<IBridge>());

        // Assert
        result.Should().Be(-1);
    }

    [Test]
    public void GetDownBlockedBetween_WhenCacheHit_ShouldReturnCachedValue()
    {
        // Arrange
        var mockIslandDown = new Mock<IIsland>(MockBehavior.Strict);
        mockIslandDown.Setup(i => i.Y).Returns(3);

        var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
        mockIsland.Setup(i => i.X).Returns(1);
        mockIsland.Setup(i => i.Y).Returns(1);
        mockIsland.Setup(i => i.IslandDown).Returns(mockIslandDown.Object);

        var field = new int[][] {
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0],
            [0, 0, 0]
        };
        var bridges = new List<IBridge>();

        // Act - Call twice to test cache
        var result1 = blockDetectionService.GetDownBlockedBetween(mockIsland.Object, field, bridges);
        var result2 = blockDetectionService.GetDownBlockedBetween(mockIsland.Object, field, bridges);

        // Assert - Both calls should return the same result
        result1.Should().Be(result2);
    }

    #endregion

    #region GetRightBlockedBetween Tests

    [Test]
    public void GetRightBlockedBetween_WhenIslandRightIsNull_ShouldReturnNegativeOne()
    {
        // Arrange
        var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
        mockIsland.Setup(i => i.IslandRight).Returns((IIsland?)null);
        var field = new int[][] { [0, 0, 0] };

        // Act
        var result = blockDetectionService.GetRightBlockedBetween(mockIsland.Object, field, new List<IBridge>());

        // Assert
        result.Should().Be(-1);
    }

    [Test]
    public void GetRightBlockedBetween_WhenCacheHit_ShouldReturnCachedValue()
    {
        // Arrange
        var mockIslandRight = new Mock<IIsland>(MockBehavior.Strict);
        mockIslandRight.Setup(i => i.X).Returns(3);

        var mockIsland = new Mock<IIsland>(MockBehavior.Strict);
        mockIsland.Setup(i => i.X).Returns(1);
        mockIsland.Setup(i => i.Y).Returns(1);
        mockIsland.Setup(i => i.IslandRight).Returns(mockIslandRight.Object);

        var field = new int[][] {
            [0, 0, 0, 0],
            [0, 0, 0, 0],
            [0, 0, 0, 0]
        };
        var bridges = new List<IBridge>();

        // Act - Call twice to test cache
        var result1 = blockDetectionService.GetRightBlockedBetween(mockIsland.Object, field, bridges);
        var result2 = blockDetectionService.GetRightBlockedBetween(mockIsland.Object, field, bridges);

        // Assert - Both calls should return the same result
        result1.Should().Be(result2);
    }

    #endregion

    #region ClearCaches Tests

    [Test]
    public void ClearCaches_WhenCalled_ShouldNotThrow()
    {
        // Act & Assert
        var act = () => blockDetectionService.ClearCaches();
        act.Should().NotThrow();
    }

    #endregion
}
