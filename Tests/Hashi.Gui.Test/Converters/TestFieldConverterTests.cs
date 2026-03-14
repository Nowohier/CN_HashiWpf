using FluentAssertions;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Converters;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Hashi.Gui.Test.Converters;

/// <summary>
/// Unit tests for <see cref="TestFieldConverter"/> class.
/// </summary>
[TestFixture]
public class TestFieldConverterTests
{
    private Mock<ISolutionProvider> solutionProviderMock;
    private Func<IReadOnlyList<int[]>?, IReadOnlyList<IBridgeCoordinates>?, string?, ISolutionProvider> factory;

    private IReadOnlyList<int[]>? capturedField;
    private IReadOnlyList<IBridgeCoordinates>? capturedBridges;
    private string? capturedName;

    [SetUp]
    public void SetUp()
    {
        solutionProviderMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        capturedField = null;
        capturedBridges = null;
        capturedName = null;

        factory = (field, bridges, name) =>
        {
            capturedField = field;
            capturedBridges = bridges;
            capturedName = name;
            return solutionProviderMock.Object;
        };
    }

    #region Null Guard Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenAllIslandEnumerableIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => TestFieldConverter.ConvertIslandsToSolutionProvider(
            null!, "test", factory);
        action.Should().Throw<ArgumentNullException>().WithParameterName("allIslandEnumerable");
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenSolutionNameIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var islands = new List<IIslandViewModel>();

        // Act & Assert
        var action = () => TestFieldConverter.ConvertIslandsToSolutionProvider(
            islands, null!, factory);
        action.Should().Throw<ArgumentNullException>().WithParameterName("solutionName");
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenSolutionProviderFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var islands = new List<IIslandViewModel>();

        // Act & Assert
        var action = () => TestFieldConverter.ConvertIslandsToSolutionProvider(
            islands, "test", null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("solutionProviderFactory");
    }

    #endregion

    #region Field Dimension Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenValidIslands_ShouldCreateCorrectFieldDimensions()
    {
        // Arrange
        var island1 = CreateIslandMock(0, 0, 2);
        var island2 = CreateIslandMock(3, 4, 3);
        var islands = new List<IIslandViewModel> { island1.Object, island2.Object };

        // Act
        TestFieldConverter.ConvertIslandsToSolutionProvider(islands, "test", factory);

        // Assert
        capturedField.Should().NotBeNull();
        capturedField.Should().HaveCount(5); // maxY + 1 = 4 + 1
        capturedField![0].Should().HaveCount(4); // maxX + 1 = 3 + 1
    }

    #endregion

    #region MaxConnections Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenIslandsHaveConnections_ShouldPopulateMaxConnections()
    {
        // Arrange
        var island1 = CreateIslandMock(0, 0, 2);
        var island2 = CreateIslandMock(2, 0, 3);
        var islands = new List<IIslandViewModel> { island1.Object, island2.Object };

        // Act
        TestFieldConverter.ConvertIslandsToSolutionProvider(islands, "test", factory);

        // Assert
        capturedField.Should().NotBeNull();
        capturedField![0][0].Should().Be(2);
        capturedField[0][2].Should().Be(3);
    }

    #endregion

    #region Bridge Coordinate Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenIslandsHaveBridges_ShouldCreateBridgeCoordinates()
    {
        // Arrange
        var connectionPoint = new Mock<IHashiPoint>(MockBehavior.Strict);
        connectionPoint.Setup(p => p.X).Returns(2);
        connectionPoint.Setup(p => p.Y).Returns(0);

        var island1 = CreateIslandMock(0, 0, 1,
            new ObservableCollection<IHashiPoint> { connectionPoint.Object });
        var island2 = CreateIslandMock(2, 0, 1);
        var islands = new List<IIslandViewModel> { island1.Object, island2.Object };

        // Act
        TestFieldConverter.ConvertIslandsToSolutionProvider(islands, "test", factory);

        // Assert
        capturedBridges.Should().NotBeNull();
        capturedBridges.Should().HaveCount(1);
        capturedBridges![0].Location1.Should().Be(new Point(0, 0));
        capturedBridges[0].Location2.Should().Be(new Point(2, 0));
        capturedBridges[0].AmountBridges.Should().Be(1);
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenDuplicateBridges_ShouldDeduplicateReversePairs()
    {
        // Arrange — island1 connects to island2 and island2 connects to island1
        var connectionTo2 = new Mock<IHashiPoint>(MockBehavior.Strict);
        connectionTo2.Setup(p => p.X).Returns(2);
        connectionTo2.Setup(p => p.Y).Returns(0);

        var connectionTo1 = new Mock<IHashiPoint>(MockBehavior.Strict);
        connectionTo1.Setup(p => p.X).Returns(0);
        connectionTo1.Setup(p => p.Y).Returns(0);

        var island1 = CreateIslandMock(0, 0, 1,
            new ObservableCollection<IHashiPoint> { connectionTo2.Object });
        var island2 = CreateIslandMock(2, 0, 1,
            new ObservableCollection<IHashiPoint> { connectionTo1.Object });
        var islands = new List<IIslandViewModel> { island1.Object, island2.Object };

        // Act
        TestFieldConverter.ConvertIslandsToSolutionProvider(islands, "test", factory);

        // Assert — reverse pair should be deduplicated
        capturedBridges.Should().HaveCount(1);
    }

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenNoConnections_ShouldReturnEmptyBridgeList()
    {
        // Arrange
        var island1 = CreateIslandMock(0, 0, 0);
        var island2 = CreateIslandMock(1, 1, 0);
        var islands = new List<IIslandViewModel> { island1.Object, island2.Object };

        // Act
        TestFieldConverter.ConvertIslandsToSolutionProvider(islands, "test", factory);

        // Assert
        capturedBridges.Should().NotBeNull();
        capturedBridges.Should().BeEmpty();
    }

    #endregion

    #region Factory Invocation Tests

    [Test]
    public void ConvertIslandsToSolutionProvider_WhenCalled_ShouldPassSolutionNameToFactory()
    {
        // Arrange
        var island = CreateIslandMock(0, 0, 1);
        var islands = new List<IIslandViewModel> { island.Object };

        // Act
        TestFieldConverter.ConvertIslandsToSolutionProvider(islands, "MySolution", factory);

        // Assert
        capturedName.Should().Be("MySolution");
    }

    #endregion

    #region Helper Methods

    private static Mock<IIslandViewModel> CreateIslandMock(int x, int y, int maxConnections,
        ObservableCollection<IHashiPoint>? connections = null)
    {
        var coordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        coordinatesMock.Setup(c => c.X).Returns(x);
        coordinatesMock.Setup(c => c.Y).Returns(y);

        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        islandMock.Setup(i => i.Coordinates).Returns(coordinatesMock.Object);
        islandMock.Setup(i => i.MaxConnections).Returns(maxConnections);
        islandMock.Setup(i => i.AllConnections)
            .Returns(connections ?? new ObservableCollection<IHashiPoint>());

        return islandMock;
    }

    #endregion
}
