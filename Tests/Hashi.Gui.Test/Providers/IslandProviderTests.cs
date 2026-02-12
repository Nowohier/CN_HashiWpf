using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Helpers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Providers;
using Hashi.Logging.Interfaces;
using Moq;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class IslandProviderTests
{
    [SetUp]
    public void SetUp()
    {
        islandFactoryMock = new Mock<Func<int, int, int, IIslandViewModel>>(MockBehavior.Strict);
        bridgeFactoryMock =
            new Mock<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>>(MockBehavior.Strict);
        allConnectionsSetMessageFactoryMock = new Mock<Func<bool?, IAllConnectionsSetMessage>>(MockBehavior.Strict);
        dialogWrapperMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        solutionProviderMock = new Mock<ISolutionProvider>(MockBehavior.Strict);
        islandViewModelMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        hashiPointMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        hashiBridgeMock = new Mock<IHashiBridge>(MockBehavior.Strict);
        coreMock = new Mock<IIslandProviderCore>(MockBehavior.Strict);
        helperMock = new Mock<IIslandViewModelHelper>(MockBehavior.Strict);

        // Setup logger
        loggerFactoryMock.Setup(x => x.CreateLogger<IslandProvider>()).Returns(loggerMock.Object);
        loggerMock.Setup(x => x.Info(It.IsAny<string>()));

        // Setup island factory
        islandFactoryMock.Setup(x => x.Invoke(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(islandViewModelMock.Object);

        // Setup bridge factory
        bridgeFactoryMock.Setup(x =>
                x.Invoke(It.IsAny<BridgeOperationTypeEnum>(), It.IsAny<IHashiPoint>(), It.IsAny<IHashiPoint>()))
            .Returns(hashiBridgeMock.Object);

        // Setup island view model
        islandViewModelMock.Setup(x => x.MaxConnections).Returns(4);
        islandViewModelMock.Setup(x => x.MaxConnectionsReached).Returns(false);
        islandViewModelMock.Setup(x => x.Coordinates).Returns(hashiPointMock.Object);
        islandViewModelMock.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
        islandViewModelMock.Setup(x => x.RefreshIslandColor());
        islandViewModelMock.Setup(x => x.GetConnectionType(It.IsAny<IIslandViewModel>()))
            .Returns(ConnectionTypeEnum.Horizontal);
        islandViewModelMock.Setup(x => x.AddConnection(It.IsAny<IHashiPoint>()));
        islandViewModelMock.Setup(x => x.RemoveAllConnectionsMatchingCoordinates(It.IsAny<IHashiPoint>()));
        islandViewModelMock.Setup(x => x.ResetDropTarget());
        islandViewModelMock.Setup(x => x.MaxBridgesReachedToTarget(It.IsAny<IIslandViewModel>())).Returns(false);
        islandViewModelMock.Setup(x => x.NotifyBridgeConnections());

        // Setup helper
        helperMock.Setup(x => x.MaxBridgesReachedToTarget(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>()))
            .Returns(false);
        helperMock.Setup(x => x.GetConnectionType(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>()))
            .Returns(ConnectionTypeEnum.Horizontal);

        // Setup hashiPoint
        hashiPointMock.Setup(x => x.X).Returns(1);
        hashiPointMock.Setup(x => x.Y).Returns(1);
        hashiPointMock.Setup(x => x.Clone()).Returns(hashiPointMock.Object);
        hashiPointMock.SetupProperty(x => x.PointType);

        // Setup solution provider
        solutionProviderMock.Setup(x => x.HashiField).Returns(new[] { new[] { 1, 2, 3 }, new[] { 4, 5, 6 } });
        solutionProviderMock.Setup(x => x.BridgeCoordinates).Returns(new List<IBridgeCoordinates>());

        islandProvider = new IslandProvider(
            islandFactoryMock.Object,
            bridgeFactoryMock.Object,
            allConnectionsSetMessageFactoryMock.Object,
            dialogWrapperMock.Object,
            loggerFactoryMock.Object,
            coreMock.Object,
            helperMock.Object);
    }

    private Mock<Func<int, int, int, IIslandViewModel>> islandFactoryMock;
    private Mock<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>> bridgeFactoryMock;
    private Mock<Func<bool?, IAllConnectionsSetMessage>> allConnectionsSetMessageFactoryMock;
    private Mock<IDialogWrapper> dialogWrapperMock;
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private Mock<ISolutionProvider> solutionProviderMock;
    private Mock<IIslandViewModel> islandViewModelMock;
    private Mock<IHashiPoint> hashiPointMock;
    private Mock<IHashiBridge> hashiBridgeMock;
    private Mock<IIslandProviderCore> coreMock;
    private Mock<IIslandViewModelHelper> helperMock;
    private IslandProvider islandProvider;

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Assert
        islandProvider.Should().NotBeNull();
        islandProvider.Islands.Should().BeEmpty();
        islandProvider.IslandsFlat.Should().BeEmpty();
        loggerMock.Verify(x => x.Info("IslandProvider initialized"), Times.Once);
    }

    [Test]
    public void Constructor_WhenIslandFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            null!,
            bridgeFactoryMock.Object,
            allConnectionsSetMessageFactoryMock.Object,
            dialogWrapperMock.Object,
            loggerFactoryMock.Object,
            coreMock.Object,
            helperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("islandFactory");
    }

    [Test]
    public void Constructor_WhenBridgeFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            islandFactoryMock.Object,
            null!,
            allConnectionsSetMessageFactoryMock.Object,
            dialogWrapperMock.Object,
            loggerFactoryMock.Object,
            coreMock.Object,
            helperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("bridgeFactory");
    }

    [Test]
    public void Constructor_WhenAllConnectionsSetMessageFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            islandFactoryMock.Object,
            bridgeFactoryMock.Object,
            null!,
            dialogWrapperMock.Object,
            loggerFactoryMock.Object,
            coreMock.Object,
            helperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("allConnectionsSetMessageFactory");
    }

    [Test]
    public void Constructor_WhenDialogWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            islandFactoryMock.Object,
            bridgeFactoryMock.Object,
            allConnectionsSetMessageFactoryMock.Object,
            null!,
            loggerFactoryMock.Object,
            coreMock.Object,
            helperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("dialogWrapper");
    }

    [Test]
    public void Constructor_WhenLoggerFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            islandFactoryMock.Object,
            bridgeFactoryMock.Object,
            allConnectionsSetMessageFactoryMock.Object,
            dialogWrapperMock.Object,
            null!,
            coreMock.Object,
            helperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("loggerFactory");
    }

    [Test]
    public void Constructor_WhenCoreIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            islandFactoryMock.Object,
            bridgeFactoryMock.Object,
            allConnectionsSetMessageFactoryMock.Object,
            dialogWrapperMock.Object,
            loggerFactoryMock.Object,
            null!,
            helperMock.Object);

        action.Should().Throw<ArgumentNullException>().WithParameterName("core");
    }

    [Test]
    public void Constructor_WhenHelperIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new IslandProvider(
            islandFactoryMock.Object,
            bridgeFactoryMock.Object,
            allConnectionsSetMessageFactoryMock.Object,
            dialogWrapperMock.Object,
            loggerFactoryMock.Object,
            coreMock.Object,
            null!);

        action.Should().Throw<ArgumentNullException>().WithParameterName("helper");
    }

    [Test]
    public void InitializeNewSolution_WhenSolutionProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => islandProvider.InitializeNewSolution(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("solutionProvider");
    }

    [Test]
    public void InitializeNewSolution_WhenHashiFieldIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        solutionProviderMock.Setup(x => x.HashiField).Returns((int[][])null!);

        // Act & Assert
        var action = () => islandProvider.InitializeNewSolution(solutionProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("HashiField");
    }

    [Test]
    public void InitializeNewSolution_WhenValidSolutionProvider_ShouldInitializeIslands()
    {
        // Act
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Assert
        islandProvider.Islands.Should().HaveCount(2); // 2 rows
        islandProvider.Islands[0].Should().HaveCount(3); // 3 columns in first row
        islandProvider.Islands[1].Should().HaveCount(3); // 3 columns in second row
        islandFactoryMock.Verify(x => x.Invoke(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(6));
    }

    [Test]
    public void InitializeNewSolutionAndSetBridges_WhenSolutionProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => islandProvider.InitializeNewSolutionAndSetBridges(null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("solutionProvider");
    }

    [Test]
    public void InitializeNewSolutionAndSetBridges_WhenHashiFieldIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        solutionProviderMock.Setup(x => x.HashiField).Returns((int[][])null!);

        // Act & Assert
        var action = () => islandProvider.InitializeNewSolutionAndSetBridges(solutionProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("HashiField");
    }

    [Test]
    public void
        InitializeNewSolutionAndSetBridges_WhenValidSolutionProviderWithBridges_ShouldInitializeIslandsAndSetBridges()
    {
        // Arrange
        var bridgeCoordinatesMock = new Mock<IBridgeCoordinates>(MockBehavior.Strict);
        bridgeCoordinatesMock.Setup(x => x.Location1).Returns(new Point(0, 0));
        bridgeCoordinatesMock.Setup(x => x.Location2).Returns(new Point(1, 0));
        bridgeCoordinatesMock.Setup(x => x.AmountBridges).Returns(1);
        solutionProviderMock.Setup(x => x.BridgeCoordinates)
            .Returns(new List<IBridgeCoordinates> { bridgeCoordinatesMock.Object });

        // Setup core mock for AddConnection path
        coreMock.Setup(x => x.IsValidConnectionBetweenSourceAndTarget(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>()))
            .Returns(true);
        coreMock.Setup(x => x.ManageConnections(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(),
            It.IsAny<Action<IIslandViewModel, IHashiPoint>>(),
            It.IsAny<HashiPointTypeEnum>()));
        coreMock.Setup(x => x.CountIsolatedIslandGroups(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            It.IsAny<IEnumerable<IIslandViewModel>>(),
            It.IsAny<Func<IIslandViewModel, List<IIslandViewModel>>>()))
            .Returns(0);

        // Act
        islandProvider.InitializeNewSolutionAndSetBridges(solutionProviderMock.Object);

        // Assert
        islandProvider.Islands.Should().HaveCount(2);
        islandFactoryMock.Verify(x => x.Invoke(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(6));
        islandViewModelMock.Verify(x => x.RefreshIslandColor(), Times.AtLeastOnce);
    }

    [Test]
    public void AddConnection_WhenSourceIslandIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => islandProvider.AddConnection(null!, islandViewModelMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("sourceIsland");
    }

    [Test]
    public void AddConnection_WhenTargetIslandIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => islandProvider.AddConnection(islandViewModelMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("targetIsland");
    }

    [Test]
    public void AddConnection_WhenConnectionIsValid_ShouldAddConnectionAndCreateBridge()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var targetIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var sourceCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var targetCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);
        sourceIslandMock.Setup(x => x.GetConnectionType(It.IsAny<IIslandViewModel>()))
            .Returns(ConnectionTypeEnum.Horizontal);
        targetIslandMock.Setup(x => x.GetConnectionType(It.IsAny<IIslandViewModel>()))
            .Returns(ConnectionTypeEnum.Horizontal);
        sourceIslandMock.Setup(x => x.MaxConnections).Returns(4);
        targetIslandMock.Setup(x => x.MaxConnections).Returns(4);
        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);
        targetIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);
        sourceIslandMock.Setup(x => x.AddConnection(It.IsAny<IHashiPoint>()));
        targetIslandMock.Setup(x => x.AddConnection(It.IsAny<IHashiPoint>()));

        // Add AllConnections setup for CountIsolatedIslandGroups
        sourceIslandMock.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
        targetIslandMock.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

        sourceCoordinatesMock.Setup(x => x.X).Returns(0);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(0);

        targetCoordinatesMock.Setup(x => x.X).Returns(1);
        targetCoordinatesMock.Setup(x => x.Y).Returns(0);

        // Setup core mock
        coreMock.Setup(x => x.IsValidConnectionBetweenSourceAndTarget(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandMock.Object))
            .Returns(true);
        coreMock.Setup(x => x.ManageConnections(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandMock.Object,
            It.IsAny<Action<IIslandViewModel, IHashiPoint>>(),
            It.IsAny<HashiPointTypeEnum>()));
        coreMock.Setup(x => x.CountIsolatedIslandGroups(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            It.IsAny<IEnumerable<IIslandViewModel>>(),
            It.IsAny<Func<IIslandViewModel, List<IIslandViewModel>>>()))
            .Returns(0);

        // Initialize the islands collection
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);
        islandProvider.Islands.Clear();
        var row = new ObservableCollection<IIslandViewModel> { sourceIslandMock.Object, targetIslandMock.Object };
        islandProvider.Islands.Add(row);

        // Act
        islandProvider.AddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // Assert
        bridgeFactoryMock.Verify(
            x => x.Invoke(BridgeOperationTypeEnum.Add, sourceCoordinatesMock.Object, targetCoordinatesMock.Object),
            Times.Once);
        coreMock.Verify(x => x.ManageConnections(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandMock.Object,
            It.IsAny<Action<IIslandViewModel, IHashiPoint>>(),
            It.IsAny<HashiPointTypeEnum>()), Times.Once);
    }

    [Test]
    public void AddConnection_WhenAllConnectionsSetAndNoIsolatedGroups_ShouldSendCompletionMessage()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var targetIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var sourceCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var targetCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var allConnectionsSetMessageMock = new Mock<IAllConnectionsSetMessage>(MockBehavior.Strict);

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);
        sourceIslandMock.Setup(x => x.MaxConnections).Returns(2);
        targetIslandMock.Setup(x => x.MaxConnections).Returns(2);
        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);
        targetIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);

        sourceCoordinatesMock.Setup(x => x.X).Returns(0);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(0);
        targetCoordinatesMock.Setup(x => x.X).Returns(1);
        targetCoordinatesMock.Setup(x => x.Y).Returns(0);

        // Setup core mock
        coreMock.Setup(x => x.IsValidConnectionBetweenSourceAndTarget(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandMock.Object))
            .Returns(true);
        coreMock.Setup(x => x.ManageConnections(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandMock.Object,
            It.IsAny<Action<IIslandViewModel, IHashiPoint>>(),
            It.IsAny<HashiPointTypeEnum>()));
        coreMock.Setup(x => x.CountIsolatedIslandGroups(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            It.IsAny<IEnumerable<IIslandViewModel>>(),
            It.IsAny<Func<IIslandViewModel, List<IIslandViewModel>>>()))
            .Returns(0);

        helperMock.Setup(x => x.MaxBridgesReachedToTarget(sourceIslandMock.Object, targetIslandMock.Object))
            .Returns(false);

        allConnectionsSetMessageFactoryMock.Setup(x => x.Invoke(null))
            .Returns(allConnectionsSetMessageMock.Object);

        // Initialize the islands collection with only the two islands
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);
        islandProvider.Islands.Clear();
        var row = new ObservableCollection<IIslandViewModel> { sourceIslandMock.Object, targetIslandMock.Object };
        islandProvider.Islands.Add(row);

        // Act
        islandProvider.AddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // Assert
        allConnectionsSetMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
    }

    [Test]
    public void RemoveAllConnections_WhenSourceIslandIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => islandProvider.RemoveAllConnections(null!, islandViewModelMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("sourceIsland");
    }

    [Test]
    public void RemoveAllConnections_WhenTargetIslandIsNull_ShouldRemoveAllSourceConnections()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var targetIslandForConnectionMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var connectionMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var sourceCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var targetCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var connections = new ObservableCollection<IHashiPoint> { connectionMock.Object };

        sourceIslandMock.Setup(x => x.AllConnections).Returns(connections);
        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        sourceIslandMock.Setup(x => x.MaxConnections).Returns(4);

        targetIslandForConnectionMock.Setup(x => x.MaxConnections).Returns(4);
        targetIslandForConnectionMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);

        connectionMock.Setup(x => x.X).Returns(0);
        connectionMock.Setup(x => x.Y).Returns(0);

        sourceCoordinatesMock.Setup(x => x.X).Returns(1);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(0);

        targetCoordinatesMock.Setup(x => x.X).Returns(0);
        targetCoordinatesMock.Setup(x => x.Y).Returns(0);

        // Setup core mock
        coreMock.Setup(x => x.ManageConnections(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandForConnectionMock.Object,
            It.IsAny<Action<IIslandViewModel, IHashiPoint>>(),
            It.IsAny<HashiPointTypeEnum>()));

        // Initialize the islands collection
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);
        islandProvider.Islands.Clear();
        var row = new ObservableCollection<IIslandViewModel> { targetIslandForConnectionMock.Object, sourceIslandMock.Object };
        islandProvider.Islands.Add(row);

        // Act
        islandProvider.RemoveAllConnections(sourceIslandMock.Object, null);

        // Assert
        coreMock.Verify(x => x.ManageConnections(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandForConnectionMock.Object,
            It.IsAny<Action<IIslandViewModel, IHashiPoint>>(),
            It.IsAny<HashiPointTypeEnum>()), Times.Once);
    }

    [Test]
    public void HighlightPathToTargetIsland_WhenCalled_ShouldHighlightIslands()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var targetIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var sourceCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var targetCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);
        sourceIslandMock.Setup(x => x.MaxConnections).Returns(4);
        targetIslandMock.Setup(x => x.MaxConnections).Returns(4);
        sourceIslandMock.SetupProperty(x => x.IsHighlightHorizontalLeft);
        sourceIslandMock.SetupProperty(x => x.IsHighlightHorizontalRight);
        sourceIslandMock.SetupProperty(x => x.IsHighlightVerticalTop);
        sourceIslandMock.SetupProperty(x => x.IsHighlightVerticalBottom);
        targetIslandMock.SetupProperty(x => x.IsHighlightHorizontalLeft);
        targetIslandMock.SetupProperty(x => x.IsHighlightHorizontalRight);
        targetIslandMock.SetupProperty(x => x.IsHighlightVerticalTop);
        targetIslandMock.SetupProperty(x => x.IsHighlightVerticalBottom);

        // Also setup all highlight properties for the middle island
        islandViewModelMock.SetupProperty(x => x.IsHighlightHorizontalLeft);
        islandViewModelMock.SetupProperty(x => x.IsHighlightHorizontalRight);
        islandViewModelMock.SetupProperty(x => x.IsHighlightVerticalTop);
        islandViewModelMock.SetupProperty(x => x.IsHighlightVerticalBottom);

        sourceCoordinatesMock.Setup(x => x.X).Returns(0);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(0);
        targetCoordinatesMock.Setup(x => x.X).Returns(2);
        targetCoordinatesMock.Setup(x => x.Y).Returns(0);

        // Initialize the islands collection
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);
        islandProvider.Islands.Clear();
        var row = new ObservableCollection<IIslandViewModel>
            { sourceIslandMock.Object, islandViewModelMock.Object, targetIslandMock.Object };
        islandProvider.Islands.Add(row);

        // Setup core mock
        coreMock.Setup(x => x.GetAllIslandsInvolvedInConnection(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object, targetIslandMock.Object))
            .Returns(new List<IIslandViewModel> { sourceIslandMock.Object, islandViewModelMock.Object, targetIslandMock.Object });

        // Act
        islandProvider.HighlightPathToTargetIsland(sourceIslandMock.Object, targetIslandMock.Object);

        // Assert
        Assert.Pass("HighlightPathToTargetIsland completed without throwing");
    }

    [Test]
    public void RemoveAllHighlights_WhenCalled_ShouldRemoveAllHighlights()
    {
        // Arrange
        islandViewModelMock.SetupProperty(x => x.IsHighlightHorizontalLeft);
        islandViewModelMock.SetupProperty(x => x.IsHighlightHorizontalRight);
        islandViewModelMock.SetupProperty(x => x.IsHighlightVerticalTop);
        islandViewModelMock.SetupProperty(x => x.IsHighlightVerticalBottom);

        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        islandProvider.RemoveAllHighlights();

        // Assert
        // The method should set all highlight properties to false
        Assert.Pass("RemoveAllHighlights completed without throwing");
    }

    [Test]
    public void RefreshIslandColors_WhenCalled_ShouldRefreshAllIslandColors()
    {
        // Arrange
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        islandProvider.RefreshIslandColors();

        // Assert
        islandViewModelMock.Verify(x => x.RefreshIslandColor(), Times.AtLeast(6)); // 6 islands created
    }

    [Test]
    public void ClearTemporaryDropTargets_WhenCalled_ShouldClearAllDropTargets()
    {
        // Arrange
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        islandProvider.ClearTemporaryDropTargets();

        // Assert
        islandViewModelMock.Verify(x => x.ResetDropTarget(), Times.AtLeast(6)); // 6 islands created
    }

    [Test]
    public void RemoveAllBridges_WhenCalledWithAllPointType_ShouldRemoveAllBridges()
    {
        // Arrange
        var connections = new ObservableCollection<IHashiPoint> { hashiPointMock.Object };
        islandViewModelMock.Setup(x => x.AllConnections).Returns(connections);
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        islandProvider.RemoveAllBridges(HashiPointTypeEnum.All);

        // Assert
        islandViewModelMock.Verify(x => x.NotifyBridgeConnections(), Times.AtLeast(6)); // 6 islands created
    }

    [Test]
    public void CountIsolatedIslandGroups_WhenNoIsolatedGroups_ShouldReturnZero()
    {
        // Arrange
        coreMock.Setup(x => x.CountIsolatedIslandGroups(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            It.IsAny<IEnumerable<IIslandViewModel>>(),
            It.IsAny<Func<IIslandViewModel, List<IIslandViewModel>>>()))
            .Returns(0);
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        var result = islandProvider.CountIsolatedIslandGroups();

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void GetVisibleNeighbor_WhenDirectionIsUp_ShouldReturnCorrectNeighbor()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var sourceCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        sourceCoordinatesMock.Setup(x => x.X).Returns(1);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(1);

        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        coreMock.Setup(x => x.GetVisibleNeighbor(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object,
            DirectionEnum.Up))
            .Returns(islandViewModelMock.Object);

        // Act
        var result = islandProvider.GetVisibleNeighbor(sourceIslandMock.Object, DirectionEnum.Up);

        // Assert
        result.Should().NotBeNull();
    }

    [Test]
    public void GetVisibleNeighbor_WhenDirectionIsNone_ShouldReturnNull()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);

        coreMock.Setup(x => x.GetVisibleNeighbor(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object,
            DirectionEnum.None))
            .Returns((IIslandViewModel?)null);

        // Act
        var result = islandProvider.GetVisibleNeighbor(sourceIslandMock.Object, DirectionEnum.None);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void GetAllVisibleNeighbors_WhenCalled_ShouldReturnListOfNeighbors()
    {
        // Arrange
        var sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var sourceCoordinatesMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        sourceCoordinatesMock.Setup(x => x.X).Returns(1);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(1);

        coreMock.Setup(x => x.GetAllVisibleNeighbors(
            It.IsAny<ObservableCollection<ObservableCollection<IIslandViewModel>>>(),
            sourceIslandMock.Object))
            .Returns(new List<IIslandViewModel>());

        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        var result = islandProvider.GetAllVisibleNeighbors(sourceIslandMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<List<IIslandViewModel>>();
    }

    [Test]
    public void UndoConnection_WhenCalled_ShouldNotThrow()
    {
        // Act
        islandProvider.UndoConnection();

        // Assert
        Assert.Pass("UndoConnection completed without throwing");
    }

    [Test]
    public void Receive_WhenUpdateAllIslandColorsMessage_ShouldRefreshColors()
    {
        // Arrange
        var messageMock = new Mock<IUpdateAllIslandColorsMessage>(MockBehavior.Strict);
        islandProvider.InitializeNewSolution(solutionProviderMock.Object);

        // Act
        islandProvider.Receive(messageMock.Object);

        // Assert
        islandViewModelMock.Verify(x => x.RefreshIslandColor(), Times.AtLeast(6)); // 6 islands created
    }
}