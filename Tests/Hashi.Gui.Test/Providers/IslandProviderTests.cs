using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Providers;
using Hashi.Logging.Interfaces;
using Moq;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class IslandProviderTests
{
    private Mock<Func<int, int, int, IIslandViewModel>> mockIslandFactory;
    private Mock<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>> mockBridgeFactory;
    private Mock<Func<bool?, IAllConnectionsSetMessage>> mockAllConnectionsSetMessageFactory;
    private Mock<IDialogWrapper> mockDialogWrapper;
    private Mock<ILoggerFactory> mockLoggerFactory;
    private Mock<ILogger> mockLogger;
    private IslandProvider sut;

    [SetUp]
    public void SetUp()
    {
        mockIslandFactory = new Mock<Func<int, int, int, IIslandViewModel>>();
        mockBridgeFactory = new Mock<Func<BridgeOperationTypeEnum, IHashiPoint, IHashiPoint, IHashiBridge>>();
        mockAllConnectionsSetMessageFactory = new Mock<Func<bool?, IAllConnectionsSetMessage>>();
        mockDialogWrapper = new Mock<IDialogWrapper>();
        mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLogger = new Mock<ILogger>();

        mockLoggerFactory.Setup(x => x.CreateLogger<IslandProvider>()).Returns(mockLogger.Object);

        sut = new IslandProvider(
            mockIslandFactory.Object,
            mockBridgeFactory.Object,
            mockAllConnectionsSetMessageFactory.Object,
            mockDialogWrapper.Object,
            mockLoggerFactory.Object);
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new IslandProvider(
            mockIslandFactory.Object,
            mockBridgeFactory.Object,
            mockAllConnectionsSetMessageFactory.Object,
            mockDialogWrapper.Object,
            mockLoggerFactory.Object);

        // Assert
        result.Islands.Should().NotBeNull().And.BeEmpty();
        result.IslandsFlat.Should().NotBeNull().And.BeEmpty();
        mockLogger.Verify(x => x.Info("IslandProvider initialized"), Times.Once);
    }

    [Test]
    public void InitializeNewSolution_WhenCalledWithNullSolutionProvider_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.InitializeNewSolution(null!))
           .Should().Throw<ArgumentNullException>()
           .WithParameterName("solutionProvider");
    }

    [Test]
    public void InitializeNewSolution_WhenCalledWithNullHashiField_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockSolutionProvider = new Mock<ISolutionProvider>();
        mockSolutionProvider.Setup(x => x.HashiField).Returns((IReadOnlyList<int[]>)null!);

        // Act & Assert
        sut.Invoking(x => x.InitializeNewSolution(mockSolutionProvider.Object))
           .Should().Throw<ArgumentNullException>()
           .WithParameterName("HashiField");
    }

    [Test]
    public void InitializeNewSolution_WhenCalledWithValidSolutionProvider_ShouldCreateIslands()
    {
        // Arrange
        var hashiField = new List<int[]>
        {
            new int[] { 1, 0, 2 },
            new int[] { 0, 0, 0 },
            new int[] { 3, 0, 1 }
        };
        
        var mockSolutionProvider = new Mock<ISolutionProvider>();
        mockSolutionProvider.Setup(x => x.HashiField).Returns(hashiField);

        var mockIslands = new List<IIslandViewModel>();
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var mockIsland = new Mock<IIslandViewModel>();
                mockIslands.Add(mockIsland.Object);
                mockIslandFactory.Setup(x => x.Invoke(col, row, hashiField[row][col]))
                               .Returns(mockIsland.Object);
            }
        }

        // Act
        sut.InitializeNewSolution(mockSolutionProvider.Object);

        // Assert
        sut.Islands.Should().HaveCount(3);
        sut.Islands.All(row => row.Count == 3).Should().BeTrue();
        sut.IslandsFlat.Should().HaveCount(9);
        
        // Verify all islands were created with correct parameters
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                mockIslandFactory.Verify(x => x.Invoke(col, row, hashiField[row][col]), Times.Once);
            }
        }
    }

    [Test]
    public void InitializeNewSolution_WhenCalledMultipleTimes_ShouldClearPreviousIslands()
    {
        // Arrange
        var hashiField1 = new List<int[]> { new int[] { 1, 2 } };
        var hashiField2 = new List<int[]> { new int[] { 3, 4, 5 } };
        
        var mockSolutionProvider1 = new Mock<ISolutionProvider>();
        mockSolutionProvider1.Setup(x => x.HashiField).Returns(hashiField1);
        
        var mockSolutionProvider2 = new Mock<ISolutionProvider>();
        mockSolutionProvider2.Setup(x => x.HashiField).Returns(hashiField2);

        SetupMockIslandFactory();

        // Act
        sut.InitializeNewSolution(mockSolutionProvider1.Object);
        var firstCount = sut.Islands.Count;
        sut.InitializeNewSolution(mockSolutionProvider2.Object);

        // Assert
        firstCount.Should().Be(1);
        sut.Islands.Should().HaveCount(1);
        sut.Islands[0].Should().HaveCount(3);
    }

    [Test]
    public void AddConnection_WhenCalledWithNullSourceIsland_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockTargetIsland = new Mock<IIslandViewModel>();

        // Act & Assert
        sut.Invoking(x => x.AddConnection(null, mockTargetIsland.Object))
           .Should().Throw<ArgumentNullException>()
           .WithParameterName("sourceIsland");
    }

    [Test]
    public void AddConnection_WhenCalledWithNullTargetIsland_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockSourceIsland = new Mock<IIslandViewModel>();

        // Act & Assert
        sut.Invoking(x => x.AddConnection(mockSourceIsland.Object, null))
           .Should().Throw<ArgumentNullException>()
           .WithParameterName("targetIsland");
    }

    [Test]
    public void AddConnection_WhenCalledWithValidIslands_ShouldCreateBridgeInHistory()
    {
        // Arrange
        var mockSourceIsland = CreateMockIslandViewModel(0, 0, 2);
        var mockTargetIsland = CreateMockIslandViewModel(1, 0, 2);
        var mockBridge = new Mock<IHashiBridge>();

        SetupValidConnection(mockSourceIsland, mockTargetIsland);
        
        mockBridgeFactory.Setup(x => x.Invoke(
            BridgeOperationTypeEnum.Add,
            mockSourceIsland.Object.Coordinates,
            mockTargetIsland.Object.Coordinates))
                        .Returns(mockBridge.Object);

        var historyCountBefore = sut.History.Count;

        // Act
        sut.AddConnection(mockSourceIsland.Object, mockTargetIsland.Object);

        // Assert
        sut.History.Should().HaveCount(historyCountBefore + 1);
        sut.History.Should().Contain(mockBridge.Object);
    }

    [Test]
    public void RemoveAllConnections_WhenCalledWithNullSourceIsland_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.RemoveAllConnections(null, null))
           .Should().Throw<ArgumentNullException>()
           .WithParameterName("sourceIsland");
    }

    [Test]
    public void RemoveAllConnections_WhenCalledWithNullTargetIsland_ShouldRemoveAllSourceConnections()
    {
        // Arrange
        var mockSourceIsland = CreateMockIslandViewModel(0, 0, 2);
        var mockConnection = new Mock<IHashiPoint>();
        var connections = new ObservableCollection<IHashiPoint> { mockConnection.Object };
        
        mockSourceIsland.Setup(x => x.AllConnections).Returns(connections);

        // Act
        sut.RemoveAllConnections(mockSourceIsland.Object, null);

        // Assert
        mockSourceIsland.Verify(x => x.RemoveAllConnectionsMatchingCoordinates(mockConnection.Object), Times.Once);
    }

    [Test]
    public void RemoveAllHighlights_WhenCalled_ShouldResetAllIslandHighlights()
    {
        // Arrange
        SetupIslandsInProvider();

        // Act
        sut.RemoveAllHighlights();

        // Assert
        foreach (var island in sut.IslandsFlat)
        {
            var mockIsland = Mock.Get(island);
            mockIsland.VerifySet(x => x.IsHighlightHorizontalLeft = false, Times.Once);
            mockIsland.VerifySet(x => x.IsHighlightHorizontalRight = false, Times.Once);
            mockIsland.VerifySet(x => x.IsHighlightVerticalTop = false, Times.Once);
            mockIsland.VerifySet(x => x.IsHighlightVerticalBottom = false, Times.Once);
        }
    }

    [Test]
    public void RefreshIslandColors_WhenCalled_ShouldCallRefreshOnAllIslands()
    {
        // Arrange
        SetupIslandsInProvider();

        // Act
        sut.RefreshIslandColors();

        // Assert
        foreach (var island in sut.IslandsFlat)
        {
            Mock.Get(island).Verify(x => x.RefreshIslandColor(), Times.Once);
        }
    }

    [Test]
    public void ClearTemporaryDropTargets_WhenCalled_ShouldCallResetDropTargetOnAllIslands()
    {
        // Arrange
        SetupIslandsInProvider();

        // Act
        sut.ClearTemporaryDropTargets();

        // Assert
        foreach (var island in sut.IslandsFlat)
        {
            Mock.Get(island).Verify(x => x.ResetDropTarget(), Times.Once);
        }
    }

    [TestCase(HashiPointTypeEnum.All)]
    [TestCase(HashiPointTypeEnum.Hint)]
    [TestCase(HashiPointTypeEnum.Test)]
    [TestCase(HashiPointTypeEnum.Normal)]
    public void RemoveAllBridges_WhenCalledWithValidPointType_ShouldRemoveCorrectConnections(HashiPointTypeEnum pointType)
    {
        // Arrange
        var mockIsland = CreateMockIslandViewModel(0, 0, 2);
        var mockConnection = new Mock<IHashiPoint>();
        mockConnection.Setup(x => x.PointType).Returns(pointType);
        
        var connections = new ObservableCollection<IHashiPoint> { mockConnection.Object };
        mockIsland.Setup(x => x.AllConnections).Returns(connections);
        
        SetupSingleIslandInProvider(mockIsland.Object);

        // Act
        sut.RemoveAllBridges(pointType);

        // Assert
        mockIsland.Verify(x => x.NotifyBridgeConnections(), Times.Once);
        connections.Should().BeEmpty();
    }

    [Test]
    public void RemoveAllBridges_WhenCalledWithInvalidPointType_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var invalidPointType = (HashiPointTypeEnum)999;

        // Act & Assert
        sut.Invoking(x => x.RemoveAllBridges(invalidPointType))
           .Should().Throw<ArgumentOutOfRangeException>()
           .WithParameterName("pointType");
    }

    [Test]
    public void Receive_WhenCalledWithUpdateAllIslandColorsMessage_ShouldRefreshAllIslandColors()
    {
        // Arrange
        SetupIslandsInProvider();
        var mockMessage = new Mock<IUpdateAllIslandColorsMessage>();

        // Act
        sut.Receive(mockMessage.Object);

        // Assert
        foreach (var island in sut.IslandsFlat)
        {
            Mock.Get(island).Verify(x => x.RefreshIslandColor(), Times.Once);
        }
    }

    // Helper methods
    private Mock<IIslandViewModel> CreateMockIslandViewModel(int x, int y, int maxConnections)
    {
        var mockIsland = new Mock<IIslandViewModel>();
        var mockCoordinates = new Mock<IHashiPoint>();
        
        mockCoordinates.Setup(c => c.X).Returns(x);
        mockCoordinates.Setup(c => c.Y).Returns(y);
        mockIsland.Setup(i => i.Coordinates).Returns(mockCoordinates.Object);
        mockIsland.Setup(i => i.MaxConnections).Returns(maxConnections);
        mockIsland.Setup(i => i.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
        
        return mockIsland;
    }

    private void SetupValidConnection(Mock<IIslandViewModel> sourceIsland, Mock<IIslandViewModel> targetIsland)
    {
        sourceIsland.Setup(x => x.MaxBridgesReachedToTarget(targetIsland.Object)).Returns(false);
        sourceIsland.Setup(x => x.AddConnection(It.IsAny<IHashiPoint>()));
        targetIsland.Setup(x => x.AddConnection(It.IsAny<IHashiPoint>()));
    }

    private void SetupMockIslandFactory()
    {
        mockIslandFactory.Setup(x => x.Invoke(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                        .Returns(() => new Mock<IIslandViewModel>().Object);
    }

    private void SetupIslandsInProvider()
    {
        var mockIslands = new List<IIslandViewModel>
        {
            CreateMockIslandViewModel(0, 0, 1).Object,
            CreateMockIslandViewModel(1, 0, 2).Object,
            CreateMockIslandViewModel(0, 1, 3).Object
        };

        var row1 = new ObservableCollection<IIslandViewModel> { mockIslands[0], mockIslands[1] };
        var row2 = new ObservableCollection<IIslandViewModel> { mockIslands[2] };
        
        sut.Islands.Add(row1);
        sut.Islands.Add(row2);
    }

    private void SetupSingleIslandInProvider(IIslandViewModel island)
    {
        var row = new ObservableCollection<IIslandViewModel> { island };
        sut.Islands.Add(row);
    }
}