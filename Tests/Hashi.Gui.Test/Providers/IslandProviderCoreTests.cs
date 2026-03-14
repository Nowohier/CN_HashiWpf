using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Core.Helpers;
using Hashi.Gui.Core.Providers;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class IslandProviderCoreTests
{
    private IslandProviderCore core;

    [SetUp]
    public void SetUp()
    {
        core = new IslandProviderCore(new IslandViewModelHelper());
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenHelperIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new IslandProviderCore(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region CheckDirection Tests

    [Test]
    public void CheckDirection_WhenDiagonal_ShouldReturnNull()
    {
        // Arrange
        var islands = CreateGrid(3, 3);
        var source = islands[0][0];

        // Act
        var result = core.CheckDirection(islands, source, 1, 1, ConnectionTypeEnum.Diagonal);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void CheckDirection_WhenNeighborExists_ShouldReturnNeighbor()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false), (2, 0, 2, false) });
        var source = islands[0][0];

        // Act
        var result = core.CheckDirection(islands, source, 1, 0, ConnectionTypeEnum.Horizontal);

        // Assert
        result.Should().NotBeNull();
        result!.Coordinates.X.Should().Be(2);
    }

    [Test]
    public void CheckDirection_WhenNoNeighborInDirection_ShouldReturnNull()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false) });
        var source = islands[0][0];

        // Act
        var result = core.CheckDirection(islands, source, 1, 0, ConnectionTypeEnum.Horizontal);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetAllVisibleNeighbors Tests

    [Test]
    public void GetAllVisibleNeighbors_ShouldReturnAllDirections()
    {
        // Arrange — center island with neighbors in all 4 directions
        var islands = CreateGrid(3, 3, new[] { (1, 0, 1, false), (0, 1, 1, false), (2, 1, 1, false), (1, 2, 1, false), (1, 1, 2, false) });
        var source = islands[1][1];

        // Act
        var result = core.GetAllVisibleNeighbors(islands, source);

        // Assert
        result.Should().HaveCount(4);
    }

    #endregion

    #region IsValidConnectionBetweenSourceAndTarget Tests

    [Test]
    public void IsValidConnectionBetweenSourceAndTarget_WhenSourceIsNull_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(1, 1);

        // Act
        var result = core.IsValidConnectionBetweenSourceAndTarget(islands, null, islands[0][0]);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidConnectionBetweenSourceAndTarget_WhenTargetIsNull_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(1, 1);

        // Act
        var result = core.IsValidConnectionBetweenSourceAndTarget(islands, islands[0][0], null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidConnectionBetweenSourceAndTarget_WhenSameIsland_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(1, 1, new[] { (0, 0, 2, false) });
        var source = islands[0][0];

        // Act
        var result = core.IsValidConnectionBetweenSourceAndTarget(islands, source, source);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidConnectionBetweenSourceAndTarget_WhenDiagonal_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(3, 3, new[] { (0, 0, 2, false), (2, 2, 2, false) });
        var source = islands[0][0];
        var target = islands[2][2];

        // Act
        var result = core.IsValidConnectionBetweenSourceAndTarget(islands, source, target);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidConnectionBetweenSourceAndTarget_WhenMaxedSource_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, true), (2, 0, 2, false) });
        var source = islands[0][0];
        var target = islands[0][2];

        // Act
        var result = core.IsValidConnectionBetweenSourceAndTarget(islands, source, target);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsValidConnectionBetweenSourceAndTarget_WhenValid_ShouldReturnTrue()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false), (2, 0, 2, false) });
        var source = islands[0][0];
        var target = islands[0][2];

        // Act
        var result = core.IsValidConnectionBetweenSourceAndTarget(islands, source, target);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region IsIslandInBetweenSourceAndTarget Tests

    [Test]
    public void IsIslandInBetweenSourceAndTarget_WhenIslandBetween_ShouldReturnTrue()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false), (1, 0, 1, false), (2, 0, 2, false) });
        var source = islands[0][0];
        var target = islands[0][2];

        // Act
        var result = core.IsIslandInBetweenSourceAndTarget(islands, source, target);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsIslandInBetweenSourceAndTarget_WhenNoIslandBetween_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false), (2, 0, 2, false) });
        var source = islands[0][0];
        var target = islands[0][2];

        // Act
        var result = core.IsIslandInBetweenSourceAndTarget(islands, source, target);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsIslandInBetweenSourceAndTarget_WhenAdjacentIslands_ShouldReturnFalse()
    {
        // Arrange
        var islands = CreateGrid(2, 1, new[] { (0, 0, 2, false), (1, 0, 2, false) });
        var source = islands[0][0];
        var target = islands[0][1];

        // Act
        var result = core.IsIslandInBetweenSourceAndTarget(islands, source, target);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region ManageConnections Tests

    [Test]
    public void ManageConnections_WhenSourceIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var islands = CreateGrid(1, 1);

        // Act & Assert
        var action = () => core.ManageConnections(islands, null!, islands[0][0], (_, _) => { });
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ManageConnections_WhenTargetIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var islands = CreateGrid(1, 1);

        // Act & Assert
        var action = () => core.ManageConnections(islands, islands[0][0], null!, (_, _) => { });
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ManageConnections_WhenDiagonal_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var islands = CreateGrid(3, 3, new[] { (0, 0, 2, false), (2, 2, 2, false) });

        // Act & Assert
        var action = () => core.ManageConnections(islands, islands[0][0], islands[2][2], (_, _) => { });
        action.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void ManageConnections_WhenValidHorizontal_ShouldInvokeAction()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false), (2, 0, 2, false) });
        var actionCalls = new List<(IIslandViewModel island, IHashiPoint point)>();

        // Act
        core.ManageConnections(islands, islands[0][0], islands[0][2],
            (island, point) => actionCalls.Add((island, point)));

        // Assert
        actionCalls.Should().NotBeEmpty();
    }

    #endregion

    #region CountIsolatedIslandGroups Tests

    [Test]
    public void CountIsolatedIslandGroups_WhenNoConnections_ShouldReturnZero()
    {
        // Arrange
        var islands = CreateGrid(3, 1, new[] { (0, 0, 2, false), (2, 0, 2, false) });
        var islandsFlat = islands.SelectMany(r => r);

        // Act
        var result = core.CountIsolatedIslandGroups(islands, islandsFlat,
            island => core.GetAllVisibleNeighbors(islands, island));

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Helper Methods

    private static ObservableCollection<ObservableCollection<IIslandViewModel>> CreateGrid(
        int width, int height, (int x, int y, int maxConn, bool maxReached)[]? islandDefs = null)
    {
        var grid = new ObservableCollection<ObservableCollection<IIslandViewModel>>();

        for (var y = 0; y < height; y++)
        {
            var row = new ObservableCollection<IIslandViewModel>();
            for (var x = 0; x < width; x++)
            {
                var def = islandDefs?.FirstOrDefault(d => d.x == x && d.y == y);
                var maxConn = def?.maxConn ?? 0;
                var maxReached = def?.maxReached ?? false;
                row.Add(CreateIslandViewModelMock(x, y, maxConn, maxReached));
            }
            grid.Add(row);
        }

        return grid;
    }

    private static IIslandViewModel CreateIslandViewModelMock(int x, int y, int maxConnections = 0,
        bool maxReached = false)
    {
        var coordMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        coordMock.Setup(c => c.X).Returns(x);
        coordMock.Setup(c => c.Y).Returns(y);

        var mock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        mock.Setup(m => m.Coordinates).Returns(coordMock.Object);
        mock.Setup(m => m.MaxConnections).Returns(maxConnections);
        mock.Setup(m => m.MaxConnectionsReached).Returns(maxReached);
        mock.Setup(m => m.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
        mock.Setup(m => m.BridgesLeft).Returns(new List<IHashiPoint>());
        mock.Setup(m => m.BridgesRight).Returns(new List<IHashiPoint>());
        mock.Setup(m => m.BridgesUp).Returns(new List<IHashiPoint>());
        mock.Setup(m => m.BridgesDown).Returns(new List<IHashiPoint>());

        return mock.Object;
    }

    #endregion
}
