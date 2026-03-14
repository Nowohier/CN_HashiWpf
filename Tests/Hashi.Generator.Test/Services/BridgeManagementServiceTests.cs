using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Models;
using Hashi.Generator.Services;
using Moq;

namespace Hashi.Generator.Test.Services;

[TestFixture]
public class BridgeManagementServiceTests
{
    private BridgeManagementService bridgeManagementService = null!;
    private Mock<Func<IIsland, IIsland, int, IBridge>> bridgeFactoryMock = null!;
    private BlockDetectionService blockDetectionService = null!;

    [SetUp]
    public void Setup()
    {
        bridgeFactoryMock = new Mock<Func<IIsland, IIsland, int, IBridge>>(MockBehavior.Strict);
        bridgeFactoryMock.Setup(f => f(It.IsAny<IIsland>(), It.IsAny<IIsland>(), It.IsAny<int>()))
            .Returns((IIsland i1, IIsland i2, int count) => new Bridge(i1, i2, count));

        blockDetectionService = new BlockDetectionService();
        bridgeManagementService = new BridgeManagementService(bridgeFactoryMock.Object, blockDetectionService);
    }

    #region Shuffle Tests

    [Test]
    public void Shuffle_WhenCalledWithArray_ShouldPreserveElements()
    {
        // Arrange
        var originalArray = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var testArray = originalArray.ToArray();

        // Act
        BridgeManagementService.Shuffle(testArray);

        // Assert
        testArray.Should().BeEquivalentTo(originalArray);
        testArray.Length.Should().Be(originalArray.Length);
    }

    [Test]
    public void Shuffle_WhenCalledWithSingleElementArray_ShouldNotThrow()
    {
        // Arrange
        var array = new[] { 42 };

        // Act & Assert
        var act = () => BridgeManagementService.Shuffle(array);
        act.Should().NotThrow();
        array[0].Should().Be(42);
    }

    [Test]
    public void Shuffle_WhenCalledWithEmptyArray_ShouldNotThrow()
    {
        // Arrange
        var array = Array.Empty<int>();

        // Act & Assert
        var act = () => BridgeManagementService.Shuffle(array);
        act.Should().NotThrow();
    }

    [Test]
    public void Shuffle_WhenCalledWithList_ShouldPreserveElements()
    {
        // Arrange
        var originalList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var testList = new List<int>(originalList);

        // Act
        BridgeManagementService.Shuffle(testList);

        // Assert
        testList.Should().BeEquivalentTo(originalList);
        testList.Count.Should().Be(originalList.Count);
    }

    [Test]
    public void Shuffle_WhenCalledWithSingleElementList_ShouldNotThrow()
    {
        // Arrange
        var list = new List<int> { 42 };

        // Act & Assert
        var act = () => BridgeManagementService.Shuffle(list);
        act.Should().NotThrow();
        list[0].Should().Be(42);
    }

    [Test]
    public void Shuffle_WhenCalledWithEmptyList_ShouldNotThrow()
    {
        // Arrange
        var list = new List<int>();

        // Act & Assert
        var act = () => BridgeManagementService.Shuffle(list);
        act.Should().NotThrow();
    }

    #endregion

    #region SetBeta Tests

    [Test]
    public void SetBeta_WhenBetaIsZero_ShouldNotModifyBridges()
    {
        // Arrange
        var field = new int[][] { [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0] };

        // Act
        bridgeManagementService.SetBeta(field, 0, new List<IBridge>());

        // Assert
        field.SelectMany(row => row).All(cell => cell == 0).Should().BeTrue();
    }

    [Test]
    public void SetBeta_WhenBetaIsNegative_ShouldNotModifyBridges()
    {
        // Arrange
        var field = new int[][] { [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0], [0, 0, 0, 0, 0] };

        // Act
        bridgeManagementService.SetBeta(field, -10, new List<IBridge>());

        // Assert
        field.SelectMany(row => row).All(cell => cell == 0).Should().BeTrue();
    }

    [Test]
    public void SetBeta_WhenNoBridgesExist_ShouldHandleGracefully()
    {
        // Arrange
        var field = new int[][] { [0, 0, 0] };

        // Act & Assert
        var act = () => bridgeManagementService.SetBeta(field, 50, new List<IBridge>());
        act.Should().NotThrow();
        field[0].Should().BeEquivalentTo(new[] { 0, 0, 0 });
    }

    [Test]
    public void SetBeta_WhenBridgesToAddIsZero_ShouldReturn()
    {
        // Arrange
        var field = new int[][] { [0, 0, 0] };

        // Act
        var act = () => bridgeManagementService.SetBeta(field, 1, new List<IBridge>());
        act.Should().NotThrow();
        field[0].Should().BeEquivalentTo(new[] { 0, 0, 0 });
    }

    #endregion

    #region AddAdditionalBridges Tests

    [Test]
    public void AddAdditionalBridges_WhenAlphaIsZero_ShouldNotAddBridges()
    {
        // Arrange
        var field = new int[][] { [0, 0, 0], [0, 0, 0] };
        var fieldSnapshot = field.Select(row => row.ToArray()).ToArray();

        // Act
        bridgeManagementService.AddAdditionalBridges(field, 0, new List<IIsland>(), new List<IBridge>());

        // Assert
        for (var i = 0; i < field.Length; i++)
        {
            field[i].Should().BeEquivalentTo(fieldSnapshot[i]);
        }
    }

    [Test]
    public void AddAdditionalBridges_WhenTargetBridgesReached_ShouldStopAddingBridges()
    {
        // Arrange
        var field = new int[][] { [1, 0, 1], [0, 0, 0] };
        var fieldSnapshot = field.Select(row => row.ToArray()).ToArray();

        // Act
        bridgeManagementService.AddAdditionalBridges(field, 1, new List<IIsland>(), new List<IBridge>());

        // Assert
        for (var i = 0; i < field.Length; i++)
        {
            field[i].Should().BeEquivalentTo(fieldSnapshot[i]);
        }
    }

    #endregion

    #region TryAddBridge Tests

    [Test]
    public void TryAddBridge_WhenDirectionIsRight_ShouldUpdateBothSourceAndTargetFieldCells()
    {
        // Arrange
        var sourceIsland = new Island(2, 0, 0);
        var targetIsland = new Island(2, 0, 3);
        var islands = new List<IIsland> { sourceIsland, targetIsland };

        var field = new int[][]
        {
            [2, 0, 0, 2]
        };

        sourceIsland.SetAllNeighbors(field, islands);
        targetIsland.SetAllNeighbors(field, islands);

        var bridges = new List<IBridge>();

        // Act
        var result = bridgeManagementService.TryAddBridge(sourceIsland, DirectionEnum.Right, field, bridges);

        // Assert
        result.Should().BeTrue();
        field[0][0].Should().Be(3, "source island field cell should be incremented");
        field[0][3].Should().Be(3, "target island field cell should be incremented");
    }

    [Test]
    public void TryAddBridge_WhenDirectionIsDown_ShouldUpdateBothSourceAndTargetFieldCells()
    {
        // Arrange
        var sourceIsland = new Island(2, 0, 0);
        var targetIsland = new Island(2, 3, 0);
        var islands = new List<IIsland> { sourceIsland, targetIsland };

        var field = new int[][]
        {
            [2],
            [0],
            [0],
            [2]
        };

        sourceIsland.SetAllNeighbors(field, islands);
        targetIsland.SetAllNeighbors(field, islands);

        var bridges = new List<IBridge>();

        // Act
        var result = bridgeManagementService.TryAddBridge(sourceIsland, DirectionEnum.Down, field, bridges);

        // Assert
        result.Should().BeTrue();
        field[0][0].Should().Be(3, "source island field cell should be incremented");
        field[3][0].Should().Be(3, "target island field cell should be incremented");
    }

    #endregion

    #region BuildBridgeCoordinates Tests

    [Test]
    public void BuildBridgeCoordinates_WhenBridgesExist_ShouldReturnCorrectCoordinates()
    {
        // Arrange
        var island1 = new Island(2, 1, 1);
        var island2 = new Island(2, 1, 3);
        var bridge = new Bridge(island1, island2, 1);
        var bridges = new List<IBridge> { bridge };

        // Act
        var result = bridgeManagementService.BuildBridgeCoordinates(bridges);

        // Assert
        result.Should().HaveCount(1);
        result[0].Location1.X.Should().Be(1);
        result[0].Location1.Y.Should().Be(1);
        result[0].Location2.X.Should().Be(3);
        result[0].Location2.Y.Should().Be(1);
        result[0].AmountBridges.Should().Be(1);
    }

    [Test]
    public void BuildBridgeCoordinates_WhenNoBridges_ShouldReturnEmptyList()
    {
        // Act
        var result = bridgeManagementService.BuildBridgeCoordinates(new List<IBridge>());

        // Assert
        result.Should().BeEmpty();
    }

    #endregion
}
