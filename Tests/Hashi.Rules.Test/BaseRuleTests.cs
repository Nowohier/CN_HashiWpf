using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Hashi.Rules.Test;

public class BaseRuleTests
{
    private Mock<IIslandProvider> islandProviderMock;
    private Mock<IRuleInfoProvider> ruleInfoProviderMock;
    private Mock<IIslandViewModel> sourceIslandMock;
    private Mock<IIslandViewModel> targetIslandMock;
    private TestableBaseRule testableBaseRule;

    [SetUp]
    public void SetUp()
    {
        ruleInfoProviderMock = new Mock<IRuleInfoProvider>();
        islandProviderMock = new Mock<IIslandProvider>();
        sourceIslandMock = new Mock<IIslandViewModel>();
        targetIslandMock = new Mock<IIslandViewModel>();
        testableBaseRule = new TestableBaseRule(ruleInfoProviderMock.Object, islandProviderMock.Object);
    }

    [TestCase(false, true, false)] // Rules not being applied
    [TestCase(true, true, false)] // Max connections reached
    [TestCase(true, false, true)] // Valid case
    public void AddConnection_ShouldBehaveCorrectlyBasedOnConditions(bool areRulesApplied, bool maxConnectionsReached,
        bool shouldAdd)
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(areRulesApplied);
        sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(maxConnectionsReached);
        sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
        targetIslandMock.Setup(x => x.AllConnections).Returns([]);

        // act
        testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // assert
        islandProviderMock.Verify(
            cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Hint),
            shouldAdd ? Times.Once() : Times.Never());
    }

    [Test]
    public void AddConnection_WhenValid_ShouldAddConnection()
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
        targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
        sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
        targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

        // act
        testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // assert
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, HashiPointTypeEnum.Hint),
            Times.Once);
    }

    [Test]
    public void AddConnections_WhenMultipleTargets_ShouldAddConnections()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object };
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
        targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
        sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
        targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

        // act
        testableBaseRule.AddConnections(sourceIslandMock.Object, targetIslands!);

        // assert
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, HashiPointTypeEnum.Hint),
            Times.Once);
    }

    [Test]
    public void AddMultipleConnections_WhenValid_ShouldAddTwoConnectionsPerTarget()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object };
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
        targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
        sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
        targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

        // act
        testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands!);

        // assert
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, HashiPointTypeEnum.Hint),
            Times.Exactly(2));
    }

    [Test]
    public void AddMissingConnectionsToOneTarget_WhenValid_ShouldAddMissingConnections()
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
        targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
        sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
        targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

        // act
        testableBaseRule.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 2);

        // assert
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, HashiPointTypeEnum.Hint),
            Times.Exactly(2));
    }

    [Test]
    public void GetConnectableNeighborsWithoutConnection_ShouldReturnOnlyConnectableNeighbors()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();

        neighborIslandMock1.Setup(x => x.MaxConnectionsReached).Returns(false);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([]);

        neighborIslandMock2.Setup(x => x.MaxConnectionsReached).Returns(true);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.GetConnectableNeighborsWithoutConnection(sourceIslandMock.Object, neighbors!);

        // assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighborIslandMock1.Object);
    }

    [TestCase(0, 0)] // No neighbors exist
    [TestCase(2, 0)] // No neighbors are connected
    [TestCase(2, 2)] // All neighbors are connected
    public void GetConnectedNeighbors_ShouldReturnCorrectNeighbors(int totalNeighbors, int connectedNeighbors)
    {
        // arrange
        var sourceCoordinatesMock = new Mock<IHashiPoint>();
        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);

        var neighbors = new List<IIslandViewModel?>();
        for (var i = 0; i < totalNeighbors; i++)
        {
            var neighborMock = new Mock<IIslandViewModel>();
            neighborMock.Setup(x => x.AllConnections).Returns(
                i < connectedNeighbors ? [sourceCoordinatesMock.Object] : []);
            neighbors.Add(neighborMock.Object);
        }

        // act
        var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors!, null);

        // assert
        result.Should().HaveCount(connectedNeighbors);
    }

    [Test]
    public void GetConnectedNeighbors_ShouldReturnAllNeighbors_WhenAllNeighborsAreConnected()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors!, null);

        // assert
        result.Should().HaveCount(2);
        result.Should().Contain(neighborIslandMock1.Object);
        result.Should().Contain(neighborIslandMock2.Object);
    }

    [Test]
    public void GetConnectedNeighbors_ShouldReturnNeighbor_WhenNeighborHasExactNumberOfConnectionsToSource()
    {
        // arrange
        var neighborIslandMock = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock.Setup(x => x.AllConnections).Returns([
            sourceCoordinatesMock.Object,
            sourceCoordinatesMock.Object
        ]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock.Object };

        // act
        var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors!, 2);

        // assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighborIslandMock.Object);
    }

    [Test]
    public void GetConnectedNeighbors_ShouldReturnOnlyConnectedNeighbors_WhenSomeNeighborsAreConnected()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors!, null);

        // assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighborIslandMock1.Object);
        result.Should().NotContain(neighborIslandMock2.Object);
    }

    [Test]
    public void AreAllNeighborsConnected_ShouldReturnTrue_WhenAllNeighborsAreConnected()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.AreAllNeighborsConnected(sourceIslandMock.Object, neighbors!);

        // assert
        result.Should().BeTrue();
    }

    [Test]
    public void AreAllNeighborsConnected_ShouldReturnFalse_WhenNotAllNeighborsAreConnected()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.AreAllNeighborsConnected(sourceIslandMock.Object, neighbors!);

        // assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetConnectedNeighbors_ShouldReturnEmptyList_WhenNoNeighborMatchesExactNumberOfConnections()
    {
        // arrange
        var neighborIslandMock = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock.Object };

        // act
        var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors!, 2);

        // assert
        result.Should().BeEmpty();
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnCorrectCount()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([
            sourceCoordinatesMock.Object, sourceCoordinatesMock.Object
        ]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, neighbors);

        // assert
        result.Should().Be(3);
    }

    [Test]
    public void AreRemainingConnectionsWithinRange_ShouldReturnTrue_WhenWithinRange()
    {
        // arrange
        sourceIslandMock.Setup(x => x.RemainingConnections).Returns(3);

        // act
        var result = testableBaseRule.AreRemainingConnectionsWithinRange(sourceIslandMock.Object, 2, 4);

        // assert
        result.Should().BeTrue();
    }

    [Test]
    public void AreRemainingConnectionsWithinRange_ShouldReturnFalse_WhenOutOfRange()
    {
        // arrange
        sourceIslandMock.Setup(x => x.RemainingConnections).Returns(5);

        // act
        var result = testableBaseRule.AreRemainingConnectionsWithinRange(sourceIslandMock.Object, 2, 4);

        // assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetMaxedOutConnectedNeighbors_ShouldReturnOnlyMaxedOutNeighbors()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock1.Setup(x => x.MaxConnectionsReached).Returns(true);
        neighborIslandMock1.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        neighborIslandMock2.Setup(x => x.MaxConnectionsReached).Returns(false);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.GetMaxedOutConnectedNeighbors(sourceIslandMock.Object, neighbors!, null);

        // assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighborIslandMock1.Object);
    }

    [Test]
    public void FinalizeConnection_ShouldRefreshColorsOfSourceAndTarget()
    {
        // arrange
        var sourceIsland = new Mock<IIslandViewModel>();
        var targetIsland = new Mock<IIslandViewModel>();

        // act
        testableBaseRule.FinalizeConnection(sourceIsland.Object, targetIsland.Object);

        // assert
        sourceIsland.Verify(x => x.RefreshIslandColor(), Times.Once);
        targetIsland.Verify(x => x.RefreshIslandColor(), Times.Once);
    }

    [Test]
    public void ExecuteAddConnection_ShouldReturnFalse_WhenMaxConnectionsReached()
    {
        // arrange
        var sourceIsland = new Mock<IIslandViewModel>();
        sourceIsland.Setup(x => x.MaxConnectionsReached).Returns(true);
        var targetIsland = new Mock<IIslandViewModel>();
        targetIsland.Setup(x => x.MaxConnectionsReached).Returns(true);

        // act
        var result = testableBaseRule.ExecuteAddConnection(sourceIsland.Object, targetIsland.Object);

        // assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_ShouldReturnFalse_WhenMaxConnectionsBetweenSourceAndTargetAreReached()
    {
        // arrange
        var sourceCoordinatesMock = new Mock<IHashiPoint>();
        var targetCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);

        sourceIslandMock.Setup(x => x.AllConnections).Returns([
            targetCoordinatesMock.Object,
            targetCoordinatesMock.Object
        ]);

        targetIslandMock.Setup(x => x.AllConnections).Returns([
            sourceCoordinatesMock.Object,
            sourceCoordinatesMock.Object
        ]);

        // act
        var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // assert
        result.Should().BeFalse();
        islandProviderMock.Verify(
            cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Hint), Times.Never);
    }

    [Test]
    public void ExecuteAddConnection_ShouldReturnTrue_WhenNoConnectionsExistBetweenSourceAndTarget()
    {
        // arrange
        var sourceCoordinatesMock = new Mock<IHashiPoint>();
        var targetCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);
        sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
        targetIslandMock.Setup(x => x.AllConnections).Returns([]);

        // act
        var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // assert
        result.Should().BeTrue();
    }

    [Test]
    public void ExecuteAddConnection_ShouldReturnTrue_WhenOneConnectionExistsBetweenSourceAndTarget()
    {
        // arrange
        var sourceCoordinatesMock = new Mock<IHashiPoint>();
        var targetCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);
        sourceIslandMock.Setup(x => x.AllConnections).Returns([targetCoordinatesMock.Object]);
        targetIslandMock.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        // act
        var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // assert
        result.Should().BeTrue();
    }

    [TestCase(0, true)] // No connections
    [TestCase(1, true)] // One connection
    [TestCase(2, false)] // Two connections
    public void ExecuteAddConnection_ShouldBehaveCorrectlyBasedOnConnectionCount(int connectionCount,
        bool expectedResult)
    {
        // arrange
        var sourceCoordinatesMock = new Mock<IHashiPoint>();
        var targetCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);

        var connections = new ObservableCollection<IHashiPoint>();
        for (var i = 0; i < connectionCount; i++) connections.Add(targetCoordinatesMock.Object);

        sourceIslandMock.Setup(x => x.AllConnections).Returns(connections);
        targetIslandMock.Setup(x => x.AllConnections).Returns(connections);

        // act
        var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object);

        // assert
        result.Should().Be(expectedResult);
    }

    [Test]
    public void EnsureRulesAreBeingApplied_ShouldReturnFalse_WhenRulesAreNotBeingApplied()
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(false);

        // act
        var result = testableBaseRule.EnsureRulesAreBeingApplied();

        // assert
        result.Should().BeFalse();
    }

    [Test]
    public void EnsureRulesAreBeingApplied_ShouldSetRuleMessage_WhenRulesAreBeingApplied()
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

        // act
        var result = testableBaseRule.EnsureRulesAreBeingApplied();

        // assert
        result.Should().BeTrue();
        ruleInfoProviderMock.VerifySet(cm => cm.RuleMessage = "Test Rule Message", Times.Once);
    }

    [Test]
    public void FinalizeConnection_ShouldNotThrow_WhenSourceOrTargetIsNull()
    {
        // act & assert
        testableBaseRule.Invoking(x => x.FinalizeConnection(null!, targetIslandMock.Object))
            .Should().NotThrow();

        testableBaseRule.Invoking(x => x.FinalizeConnection(sourceIslandMock.Object, null!))
            .Should().NotThrow();
    }

    [Test]
    public void GetConnectableNeighborsWithoutConnection_ShouldReturnEmptyList_WhenAllNeighborsAreMaxedOut()
    {
        // arrange
        var neighborIslandMock = new Mock<IIslandViewModel>();
        neighborIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);
        neighborIslandMock.Setup(x => x.AllConnections).Returns([]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock.Object };

        // act
        var result = testableBaseRule.GetConnectableNeighborsWithoutConnection(sourceIslandMock.Object, neighbors!);

        // assert
        result.Should().BeEmpty();
    }

    [Test]
    public void AddConnection_ShouldNotThrow_WhenSourceOrTargetIsNull()
    {
        // act & assert
        testableBaseRule.Invoking(x => x.AddConnection(null!, targetIslandMock.Object))
            .Should().NotThrow();

        testableBaseRule.Invoking(x => x.AddConnection(sourceIslandMock.Object, null!))
            .Should().NotThrow();
    }

    [Test]
    public void AddConnection_ShouldNotAddConnection_WhenSourceAndTargetAreSame()
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

        sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);

        targetIslandMock.Setup(x => x.AllConnections).Returns([]);

        // act
        testableBaseRule.AddConnection(sourceIslandMock.Object, sourceIslandMock.Object);

        // assert
        islandProviderMock.Verify(
            cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Hint), Times.Never);
    }

    [Test]
    public void AddConnections_ShouldNotThrow_WhenTargetListIsEmpty()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel>();
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

        // act & assert
        testableBaseRule.Invoking(x =>
                x.AddConnections(sourceIslandMock.Object, targetIslands))
            .Should().NotThrow();
    }

    [Test]
    public void AddConnections_ShouldHandleDuplicateTargets()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel> { targetIslandMock.Object, targetIslandMock.Object };
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        var hashPointMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        hashPointMock.Setup(mock => mock.X).Returns(1);
        hashPointMock.Setup(mock => mock.Y).Returns(1);

        sourceIslandMock.Setup(mock => mock.Coordinates).Returns(hashPointMock.Object);
        sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);

        targetIslandMock.Setup(x => x.AllConnections).Returns([]);

        // act
        testableBaseRule.AddConnections(sourceIslandMock.Object, targetIslands);

        // assert
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, HashiPointTypeEnum.Hint),
            Times.Exactly(2));
    }

    [Test]
    public void AddMultipleConnections_ShouldNotThrow_WhenTargetListIsEmpty()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel?>();
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

        // act & assert
        testableBaseRule.Invoking(x =>
                x.AddMultipleConnections(sourceIslandMock.Object, targetIslands!))
            .Should().NotThrow();
    }

    [Test]
    public void AddMultipleConnections_ShouldNotAdd_WhenMaxConnectionsAlreadyExist()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object };
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);

        // act
        testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands!);

        // assert
        islandProviderMock.Verify(
            cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Hint), Times.Never);
    }

    [Test]
    public void AddMissingConnectionsToOneTarget_ShouldNotThrow_WhenMissingConnectionsCountIsZero()
    {
        // act & assert
        testableBaseRule.Invoking(x =>
                x.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 0))
            .Should().NotThrow();
    }

    [Test]
    public void AddMissingConnectionsToOneTarget_ShouldNotAdd_WhenTargetHasMaxConnections()
    {
        // arrange
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        targetIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);

        // act
        testableBaseRule.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 2);

        // assert
        islandProviderMock.Verify(
            cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Hint), Times.Never);
    }

    [Test]
    public void GetConnectableNeighborsWithoutConnection_ShouldReturnEmptyList_WhenNoNeighborsExist()
    {
        // arrange
        // act
        var result =
            testableBaseRule.GetConnectableNeighborsWithoutConnection(sourceIslandMock.Object,
                new List<IIslandViewModel>());

        // assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetConnectedNeighbors_ShouldReturnEmptyList_WhenAmountConnectionsIsNotMet()
    {
        // arrange
        var neighborIslandMock = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        neighborIslandMock.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel?> { neighborIslandMock.Object };

        // act
        var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors!, 2);

        // assert
        result.Should().BeEmpty();
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnZero_WhenNoNeighborsExist()
    {
        // act
        var result =
            testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, new List<IIslandViewModel>());

        // assert
        result.Should().Be(0);
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnZero_WhenSourceIslandIsNull()
    {
        // act
        var result = testableBaseRule.CountConnectionsToNeighbors(null!, new List<IIslandViewModel>());

        // assert
        result.Should().Be(0);
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnZero_WhenNeighborsListIsNull()
    {
        // arrange, act, assert
        testableBaseRule.Invoking(x => x.CountConnectionsToNeighbors(sourceIslandMock.Object, null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnZero_WhenNeighborsHaveNoConnections()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();

        neighborIslandMock1.Setup(x => x.AllConnections).Returns([]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([]);

        var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, neighbors);

        // assert
        result.Should().Be(0);
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnZero_WhenNeighborsAreConnectedToOtherIslands()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var otherCoordinatesMock = new Mock<IHashiPoint>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();
        sourceCoordinatesMock.Setup(x => x.X).Returns(1);
        sourceCoordinatesMock.Setup(x => x.Y).Returns(1);

        neighborIslandMock1.Setup(x => x.AllConnections).Returns([otherCoordinatesMock.Object]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([otherCoordinatesMock.Object]);

        var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);

        // act
        var result = testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, neighbors);

        // assert
        result.Should().Be(0);
    }

    [Test]
    public void CountConnectionsToNeighbors_ShouldReturnCorrectCount_WhenSomeNeighborsAreConnectedToSource()
    {
        // arrange
        var neighborIslandMock1 = new Mock<IIslandViewModel>();
        var neighborIslandMock2 = new Mock<IIslandViewModel>();
        var sourceCoordinatesMock = new Mock<IHashiPoint>();

        sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);

        neighborIslandMock1.Setup(x => x.AllConnections).Returns([sourceCoordinatesMock.Object]);
        neighborIslandMock2.Setup(x => x.AllConnections).Returns([]);

        var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

        // act
        var result = testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, neighbors);

        // assert
        result.Should().Be(1);
    }


    [Test]
    public void AreRemainingConnectionsWithinRange_ShouldReturnTrue_WhenRemainingConnectionsAreAtMinValue()
    {
        // arrange
        sourceIslandMock.Setup(x => x.RemainingConnections).Returns(2);

        // act
        var result = testableBaseRule.AreRemainingConnectionsWithinRange(sourceIslandMock.Object, 2, 4);

        // assert
        result.Should().BeTrue();
    }

    [Test]
    public void AreRemainingConnectionsWithinRange_ShouldReturnTrue_WhenRemainingConnectionsAreAtMaxValue()
    {
        // arrange
        sourceIslandMock.Setup(x => x.RemainingConnections).Returns(4);

        // act
        var result = testableBaseRule.AreRemainingConnectionsWithinRange(sourceIslandMock.Object, 2, 4);

        // assert
        result.Should().BeTrue();
    }

    [Test]
    public void ExecuteAddConnection_ShouldNotThrow_WhenSourceOrTargetIsNull()
    {
        // act & assert
        testableBaseRule.Invoking(x =>
                x.ExecuteAddConnection(null!, targetIslandMock.Object))
            .Should().NotThrow();

        testableBaseRule.Invoking(x =>
                x.ExecuteAddConnection(sourceIslandMock.Object, null!))
            .Should().NotThrow();
    }

    [Test]
    public void FinalizeConnection_ShouldNotThrow_WhenSourceAndTargetAreSame()
    {
        // act & assert
        testableBaseRule.Invoking(x => x.FinalizeConnection(sourceIslandMock.Object, sourceIslandMock.Object))
            .Should().NotThrow();
    }

    [Test]
    public void AddMultipleConnections_ShouldSkipTarget_WhenTargetHasMaxConnections()
    {
        // arrange
        var targetIslands = new List<IIslandViewModel> { targetIslandMock.Object };
        ruleInfoProviderMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
        targetIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);

        // act
        testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands);

        // assert
        islandProviderMock.Verify(
            cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Hint), Times.Never);
    }

    [Test]
    public void AddMultipleConnections_ShouldAddConnectionsToSomeTargets_WhenOthersFail()
    {
        // arrange
        var targetIslandMock1 = new Mock<IIslandViewModel>();
        var targetIslandMock2 = new Mock<IIslandViewModel>();

        targetIslandMock1.Setup(x => x.MaxConnectionsReached).Returns(false);
        targetIslandMock1.Setup(x => x.AllConnections).Returns([]);
        targetIslandMock1.Setup(x => x.Coordinates).Returns(new Mock<IHashiPoint>().Object);
        targetIslandMock2.Setup(x => x.MaxConnectionsReached).Returns(true);
        targetIslandMock2.Setup(x => x.AllConnections).Returns([]);
        targetIslandMock2.Setup(x => x.Coordinates).Returns(new Mock<IHashiPoint>().Object);

        var targetIslands = new List<IIslandViewModel> { targetIslandMock1.Object, targetIslandMock2.Object };

        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(true);
        sourceIslandMock.Setup(x => x.AllConnections).Returns([]);

        // act
        testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands);

        // assert
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock1.Object, HashiPointTypeEnum.Hint),
            Times.Exactly(2));
        islandProviderMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock2.Object, HashiPointTypeEnum.Hint),
            Times.Never);
    }

    [Test]
    public void SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor_ShouldReturnValidNeighbor_WhenGroupIsIsolated()
    {
        // arrange
        var connectableNeighbor1 = new Mock<IIslandViewModel>();
        var connectableNeighbor2 = new Mock<IIslandViewModel>();
        var allNeighbors = new List<IIslandViewModel> { connectableNeighbor1.Object, connectableNeighbor2.Object };

        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);
        connectableNeighbor1.Setup(x => x.MaxConnectionsReached).Returns(true);
        connectableNeighbor2.Setup(x => x.MaxConnectionsReached).Returns(true);

        islandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(1);
        islandProviderMock.Setup(x => x.GetAllVisibleNeighbors(sourceIslandMock.Object)).Returns(allNeighbors);

        // act
        var result = testableBaseRule.SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(
            sourceIslandMock.Object,
            new List<IIslandViewModel> { connectableNeighbor1.Object, connectableNeighbor2.Object },
            allNeighbors);

        // assert
        result.Should().NotBeNull();
        islandProviderMock.Verify(x => x.AddConnection(sourceIslandMock.Object, connectableNeighbor1.Object, HashiPointTypeEnum.Test), Times.Once);
        islandProviderMock.Verify(x => x.RemoveAllBridges(HashiPointTypeEnum.Test), Times.Exactly(2));
    }

    [Test]
    public void SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor_ShouldReturnNull_WhenGroupIsNotIsolated()
    {
        // arrange
        var connectableNeighbor1 = new Mock<IIslandViewModel>();
        var connectableNeighbor2 = new Mock<IIslandViewModel>();
        var allNeighbors = new List<IIslandViewModel> { connectableNeighbor1.Object, connectableNeighbor2.Object };

        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);
        connectableNeighbor1.Setup(x => x.MaxConnectionsReached).Returns(true);
        connectableNeighbor2.Setup(x => x.MaxConnectionsReached).Returns(true);

        islandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(0);
        islandProviderMock.Setup(x => x.GetAllVisibleNeighbors(sourceIslandMock.Object)).Returns(allNeighbors);

        // act
        var result = testableBaseRule.SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(
            sourceIslandMock.Object,
            new List<IIslandViewModel> { connectableNeighbor1.Object, connectableNeighbor2.Object },
            allNeighbors);

        // assert
        result.Should().BeNull();
        islandProviderMock.Verify(x => x.AddConnection(sourceIslandMock.Object, connectableNeighbor1.Object, HashiPointTypeEnum.Test), Times.Once);
        islandProviderMock.Verify(x => x.RemoveAllBridges(HashiPointTypeEnum.Test), Times.Exactly(2));
    }

    [Test]
    public void SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor_ShouldReturnNull_WhenNoConnectableNeighborsExist()
    {
        // arrange
        var allNeighbors = new List<IIslandViewModel>();

        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);
        islandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(0);

        // act
        var result = testableBaseRule.SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(
            sourceIslandMock.Object,
            new List<IIslandViewModel>(),
            allNeighbors);

        // assert
        result.Should().BeNull();
        islandProviderMock.Verify(x => x.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), HashiPointTypeEnum.Test), Times.Never);
        islandProviderMock.Verify(x => x.RemoveAllBridges(HashiPointTypeEnum.Test), Times.Never);
    }

    [Test]
    public void SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor_ShouldRemoveTestConnections_WhenNoValidNeighborFound()
    {
        // arrange
        var connectableNeighbor = new Mock<IIslandViewModel>();
        var allNeighbors = new List<IIslandViewModel> { connectableNeighbor.Object };

        sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);
        connectableNeighbor.Setup(x => x.MaxConnectionsReached).Returns(false);

        islandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(0);

        // act
        var result = testableBaseRule.SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(
            sourceIslandMock.Object,
            new List<IIslandViewModel> { connectableNeighbor.Object },
            allNeighbors);

        // assert
        result.Should().BeNull();
        islandProviderMock.Verify(x => x.AddConnection(sourceIslandMock.Object, connectableNeighbor.Object, HashiPointTypeEnum.Test), Times.Once);
        islandProviderMock.Verify(x => x.RemoveAllBridges(HashiPointTypeEnum.Test), Times.Once);
    }

}

/// <summary>
///     A concrete implementation of BaseRule for testing purposes.
/// </summary>
[ExcludeFromCodeCoverage]
public class TestableBaseRule(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) : BaseRule(ruleInfoProvider, islandProvider)
{
    /// <summary>
    ///     Provides a test-specific rule message.
    /// </summary>
    protected override string RuleMessage => "Test Rule Message";

    /// <summary>
    ///     Allows overriding behavior for testing specific scenarios.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public bool EnsureRulesAreBeingAppliedOverride { get; set; } = true;

    /// <summary>
    ///     Overrides the EnsureRulesAreBeingApplied method for testing.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal override bool EnsureRulesAreBeingApplied()
    {
        return EnsureRulesAreBeingAppliedOverride && base.EnsureRulesAreBeingApplied();
    }

    [ExcludeFromCodeCoverage]
    public override void Define()
    {
        //Do nothing
    }
}