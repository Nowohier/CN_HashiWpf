using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;

namespace Hashi.Gui.Test.Helpers;

[TestFixture]
public class IslandViewModelHelperTests
{
    private IslandViewModelHelper helper;

    [SetUp]
    public void SetUp()
    {
        helper = new IslandViewModelHelper();
    }

    #region GetConnectionType Tests

    [Test]
    public void GetConnectionType_WhenSameX_ShouldReturnVertical()
    {
        // Arrange
        var source = CreateIslandMock(2, 0);
        var target = CreateIslandMock(2, 5);

        // Act
        var result = helper.GetConnectionType(source, target);

        // Assert
        result.Should().Be(ConnectionTypeEnum.Vertical);
    }

    [Test]
    public void GetConnectionType_WhenSameY_ShouldReturnHorizontal()
    {
        // Arrange
        var source = CreateIslandMock(0, 3);
        var target = CreateIslandMock(5, 3);

        // Act
        var result = helper.GetConnectionType(source, target);

        // Assert
        result.Should().Be(ConnectionTypeEnum.Horizontal);
    }

    [Test]
    public void GetConnectionType_WhenDifferentXAndY_ShouldReturnDiagonal()
    {
        // Arrange
        var source = CreateIslandMock(0, 0);
        var target = CreateIslandMock(3, 5);

        // Act
        var result = helper.GetConnectionType(source, target);

        // Assert
        result.Should().Be(ConnectionTypeEnum.Diagonal);
    }

    #endregion

    #region IsValidDropTarget Tests

    [Test]
    public void IsValidDropTarget_WhenTargetIsNull_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(0, 0, maxConnections: 2);

        // Act
        var result = helper.IsValidDropTarget(source, null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidDropTarget_WhenSourceMaxReached_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(0, 0, maxConnections: 2, maxReached: true);
        var target = CreateIslandMock(1, 0, maxConnections: 2);

        // Act
        var result = helper.IsValidDropTarget(source, target);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidDropTarget_WhenDiagonal_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(0, 0, maxConnections: 2);
        var target = CreateIslandMock(1, 1, maxConnections: 2);

        // Act
        var result = helper.IsValidDropTarget(source, target);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidDropTarget_WhenValid_ShouldReturnTrue()
    {
        // Arrange
        var source = CreateIslandMock(0, 0, maxConnections: 2);
        var target = CreateIslandMock(3, 0, maxConnections: 2);

        // Act
        var result = helper.IsValidDropTarget(source, target);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsValidDropTarget_WhenSourceAndTargetAreSame_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(0, 0, maxConnections: 2);

        // Act
        var result = helper.IsValidDropTarget(source, source);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidDropTarget_WhenSourceMaxConnectionsIsZero_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(0, 0, maxConnections: 0);
        var target = CreateIslandMock(1, 0, maxConnections: 2);

        // Act
        var result = helper.IsValidDropTarget(source, target);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region MaxBridgesReachedToTarget Tests

    [Test]
    public void MaxBridgesReachedToTarget_WhenTargetIsNull_ShouldReturnNull()
    {
        // Arrange
        var source = CreateIslandMock(0, 0);

        // Act
        var result = helper.MaxBridgesReachedToTarget(source, null);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void MaxBridgesReachedToTarget_WhenUnderLimit_ShouldReturnFalse()
    {
        // Arrange
        var sourceCoord = CreatePointMock(0, 0);
        var targetCoord = CreatePointMock(3, 0);
        var connection = CreatePointMock(3, 0);

        var sourceMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        sourceMock.Setup(x => x.Coordinates).Returns(sourceCoord);
        sourceMock.Setup(x => x.AllConnections)
            .Returns([connection]);

        var targetMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        targetMock.Setup(x => x.Coordinates).Returns(targetCoord);
        targetMock.Setup(x => x.AllConnections)
            .Returns([]);

        // Act
        var result = helper.MaxBridgesReachedToTarget(sourceMock.Object, targetMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void MaxBridgesReachedToTarget_WhenAtLimit_ShouldReturnTrue()
    {
        // Arrange
        var sourceCoord = CreatePointMock(0, 0);
        var targetCoord = CreatePointMock(3, 0);
        var conn1 = CreatePointMock(3, 0);
        var conn2 = CreatePointMock(3, 0);

        var sourceMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        sourceMock.Setup(x => x.Coordinates).Returns(sourceCoord);
        sourceMock.Setup(x => x.AllConnections)
            .Returns([conn1, conn2]);

        var targetMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        targetMock.Setup(x => x.Coordinates).Returns(targetCoord);
        targetMock.Setup(x => x.AllConnections)
            .Returns([]);

        // Act
        var result = helper.MaxBridgesReachedToTarget(sourceMock.Object, targetMock.Object);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetBridges Directional Tests

    [Test]
    public void GetBridgesLeft_WhenConnectionsExist_ShouldReturnLeftConnections()
    {
        // Arrange
        var coord = CreatePointMock(3, 2);
        var leftConn = CreatePointMock(1, 2);
        var rightConn = CreatePointMock(5, 2);

        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        islandMock.Setup(x => x.Coordinates).Returns(coord);
        islandMock.Setup(x => x.AllConnections)
            .Returns([leftConn, rightConn]);

        // Act
        var result = helper.GetBridgesLeft(islandMock.Object);

        // Assert
        result.Should().HaveCount(1);
        result[0].X.Should().Be(1);
    }

    [Test]
    public void GetBridgesRight_WhenConnectionsExist_ShouldReturnRightConnections()
    {
        // Arrange
        var coord = CreatePointMock(3, 2);
        var leftConn = CreatePointMock(1, 2);
        var rightConn = CreatePointMock(5, 2);

        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        islandMock.Setup(x => x.Coordinates).Returns(coord);
        islandMock.Setup(x => x.AllConnections)
            .Returns([leftConn, rightConn]);

        // Act
        var result = helper.GetBridgesRight(islandMock.Object);

        // Assert
        result.Should().HaveCount(1);
        result[0].X.Should().Be(5);
    }

    [Test]
    public void GetBridgesUp_WhenConnectionsExist_ShouldReturnUpConnections()
    {
        // Arrange
        var coord = CreatePointMock(2, 3);
        var upConn = CreatePointMock(2, 1);
        var downConn = CreatePointMock(2, 5);

        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        islandMock.Setup(x => x.Coordinates).Returns(coord);
        islandMock.Setup(x => x.AllConnections)
            .Returns([upConn, downConn]);

        // Act
        var result = helper.GetBridgesUp(islandMock.Object);

        // Assert
        result.Should().HaveCount(1);
        result[0].Y.Should().Be(1);
    }

    [Test]
    public void GetBridgesDown_WhenConnectionsExist_ShouldReturnDownConnections()
    {
        // Arrange
        var coord = CreatePointMock(2, 3);
        var upConn = CreatePointMock(2, 1);
        var downConn = CreatePointMock(2, 5);

        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        islandMock.Setup(x => x.Coordinates).Returns(coord);
        islandMock.Setup(x => x.AllConnections)
            .Returns([upConn, downConn]);

        // Act
        var result = helper.GetBridgesDown(islandMock.Object);

        // Assert
        result.Should().HaveCount(1);
        result[0].Y.Should().Be(5);
    }

    #endregion

    #region Helper Methods

    private static IIslandViewModel CreateIslandMock(int x, int y, int maxConnections = 1,
        bool maxReached = false)
    {
        var coordMock = CreatePointMock(x, y);
        var mock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        mock.Setup(m => m.Coordinates).Returns(coordMock);
        mock.Setup(m => m.MaxConnections).Returns(maxConnections);
        mock.Setup(m => m.MaxConnectionsReached).Returns(maxReached);
        return mock.Object;
    }

    private static IHashiPoint CreatePointMock(int x, int y)
    {
        var mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        mock.Setup(m => m.X).Returns(x);
        mock.Setup(m => m.Y).Returns(y);
        return mock.Object;
    }

    #endregion
}
