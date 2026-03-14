using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;
using Hashi.Generator.Services;
using Moq;

namespace Hashi.Generator.Test.Services;

[TestFixture]
public class IslandLayoutServiceTests
{
    private IslandLayoutService islandLayoutService = null!;
    private Mock<Func<int, int, int, IIsland>> islandFactoryMock = null!;
    private Mock<Func<IIsland, IIsland, int, IBridge>> bridgeFactoryMock = null!;
    private BlockDetectionService blockDetectionService = null!;

    [SetUp]
    public void Setup()
    {
        islandFactoryMock = new Mock<Func<int, int, int, IIsland>>(MockBehavior.Strict);
        bridgeFactoryMock = new Mock<Func<IIsland, IIsland, int, IBridge>>(MockBehavior.Strict);

        islandFactoryMock.Setup(f => f(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns((int bridges, int y, int x) => new Island(bridges, y, x));

        bridgeFactoryMock.Setup(f => f(It.IsAny<IIsland>(), It.IsAny<IIsland>(), It.IsAny<int>()))
            .Returns((IIsland i1, IIsland i2, int count) => new Bridge(i1, i2, count));

        blockDetectionService = new BlockDetectionService();
        islandLayoutService = new IslandLayoutService(islandFactoryMock.Object, bridgeFactoryMock.Object, blockDetectionService);
    }

    #region InitializeField Tests

    [Test]
    public void InitializeField_WhenValidDimensions_ShouldCreateCorrectSizedField()
    {
        // Arrange
        var length = 5;
        var width = 7;

        // Act
        var result = islandLayoutService.InitializeField(length, width);

        // Assert
        result.Should().HaveCount(length);
        result.All(row => row.Length == width).Should().BeTrue();
        result.SelectMany(row => row).All(cell => cell == 0).Should().BeTrue();
    }

    [Test]
    [TestCase(0, 5)]
    [TestCase(5, 0)]
    public void InitializeField_WhenZeroDimensions_ShouldCreateEmptyField(int length, int width)
    {
        // Act
        var result = islandLayoutService.InitializeField(length, width);

        // Assert
        result.Should().HaveCount(length);
        if (length > 0)
        {
            result.All(row => row.Length == width).Should().BeTrue();
        }
    }

    [Test]
    [TestCase(-1, 5)]
    [TestCase(5, -1)]
    public void InitializeField_WhenNegativeDimensions_ShouldThrowException(int length, int width)
    {
        // Act & Assert
        Action act = () => islandLayoutService.InitializeField(length, width);
        act.Should().Throw<Exception>();
    }

    #endregion

    #region HasAdjacentIsland Tests

    [Test]
    public void HasAdjacentIsland_WhenAdjacentCellHasIsland_ShouldReturnTrue()
    {
        // Arrange
        var field = new int[][]
        {
            [0, 0, 0],
            [0, 1, 2],
            [0, 0, 0]
        };

        // Act
        var result = islandLayoutService.HasAdjacentIsland(1, 1, field);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void HasAdjacentIsland_WhenNoAdjacentIsland_ShouldReturnFalse()
    {
        // Arrange
        var field = new int[][]
        {
            [0, 0, 0],
            [0, 1, 0],
            [0, 0, 0]
        };

        // Act
        var result = islandLayoutService.HasAdjacentIsland(1, 1, field);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void HasAdjacentIsland_WhenAtEdgeOfField_ShouldCheckOnlyValidPositions()
    {
        // Arrange
        var field = new int[][]
        {
            [1, 2],
            [0, 0]
        };

        // Act
        var result = islandLayoutService.HasAdjacentIsland(0, 0, field);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
