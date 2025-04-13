using FluentAssertions;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Hashi.Rules.Test
{
    public class BaseRuleTests
    {
        private Mock<IConnectionManagerViewModel> connectionManagerMock;
        private Mock<IIslandViewModel> sourceIslandMock;
        private Mock<IIslandViewModel> targetIslandMock;
        private TestableBaseRule testableBaseRule;

        [SetUp]
        public void SetUp()
        {
            connectionManagerMock = new Mock<IConnectionManagerViewModel>();
            sourceIslandMock = new Mock<IIslandViewModel>();
            targetIslandMock = new Mock<IIslandViewModel>();
            testableBaseRule = new TestableBaseRule();
        }

        [TestCase(false, true, false)] // Rules not being applied
        [TestCase(true, true, false)] // Max connections reached
        [TestCase(true, false, true)] // Valid case
        public void AddConnection_ShouldBehaveCorrectlyBasedOnConditions(bool areRulesApplied, bool maxConnectionsReached, bool shouldAdd)
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(areRulesApplied);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(maxConnectionsReached);
            sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
            targetIslandMock.Setup(x => x.AllConnections).Returns([]);

            // act
            testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(
                cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true),
                shouldAdd ? Times.Once() : Times.Never());
        }

        [Test]
        public void AddConnection_WhenValid_ShouldAddConnection()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
            targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

            // act
            testableBaseRule.AddConnection(sourceIslandMock.Object, targetIslandMock.Object,
                connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true),
                Times.Once);
        }

        [Test]
        public void AddConnections_WhenMultipleTargets_ShouldAddConnections()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object };
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
            targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

            // act
            testableBaseRule.AddConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true),
                Times.Once);
        }

        [Test]
        public void AddMultipleConnections_WhenValid_ShouldAddTwoConnectionsPerTarget()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object };
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
            targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

            // act
            testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands,
                connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true),
                Times.Exactly(2));
        }

        [Test]
        public void AddMissingConnectionsToOneTarget_WhenValid_ShouldAddMissingConnections()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(si => si.MaxConnectionsReached).Returns(false);
            targetIslandMock.Setup(ti => ti.MaxConnectionsReached).Returns(false);
            sourceIslandMock.Setup(si => si.AllConnections).Returns([]);
            targetIslandMock.Setup(ti => ti.AllConnections).Returns([]);

            // act
            testableBaseRule.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 2,
                connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true),
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
            var result = testableBaseRule.GetConnectableNeighborsWithoutConnection(sourceIslandMock.Object, neighbors);

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
            var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors, null);

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
            var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors, null);

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
            var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors, 2);

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
            var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors, null);

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
            var result = testableBaseRule.AreAllNeighborsConnected(sourceIslandMock.Object, neighbors);

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
            var result = testableBaseRule.AreAllNeighborsConnected(sourceIslandMock.Object, neighbors);

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
            var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors, 2);

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
            var result = testableBaseRule.GetMaxedOutConnectedNeighbors(sourceIslandMock.Object, neighbors, null);

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
            var result = testableBaseRule.ExecuteAddConnection(sourceIsland.Object, targetIsland.Object,
                connectionManagerMock.Object);

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
            var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object,
                connectionManagerMock.Object);

            // assert
            result.Should().BeFalse();
            connectionManagerMock.Verify(
                cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true), Times.Never);
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
            var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object,
                connectionManagerMock.Object);

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
            var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object,
                connectionManagerMock.Object);

            // assert
            result.Should().BeTrue();
        }

        [TestCase(0, true)] // No connections
        [TestCase(1, true)] // One connection
        [TestCase(2, false)] // Two connections
        public void ExecuteAddConnection_ShouldBehaveCorrectlyBasedOnConnectionCount(int connectionCount, bool expectedResult)
        {
            // arrange
            var sourceCoordinatesMock = new Mock<IHashiPoint>();
            var targetCoordinatesMock = new Mock<IHashiPoint>();

            sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
            targetIslandMock.Setup(x => x.Coordinates).Returns(targetCoordinatesMock.Object);

            var connections = new ObservableCollection<IHashiPoint>();
            for (var i = 0; i < connectionCount; i++)
            {
                connections.Add(targetCoordinatesMock.Object);
            }

            sourceIslandMock.Setup(x => x.AllConnections).Returns(connections);
            targetIslandMock.Setup(x => x.AllConnections).Returns(connections);

            // act
            var result = testableBaseRule.ExecuteAddConnection(sourceIslandMock.Object, targetIslandMock.Object, connectionManagerMock.Object);

            // assert
            result.Should().Be(expectedResult);
        }

        [Test]
        public void EnsureRulesAreBeingApplied_ShouldReturnFalse_WhenRulesAreNotBeingApplied()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(false);

            // act
            var result = testableBaseRule.EnsureRulesAreBeingApplied(connectionManagerMock.Object);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void EnsureRulesAreBeingApplied_ShouldSetRuleMessage_WhenRulesAreBeingApplied()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

            // act
            var result = testableBaseRule.EnsureRulesAreBeingApplied(connectionManagerMock.Object);

            // assert
            result.Should().BeTrue();
            connectionManagerMock.VerifySet(cm => cm.RuleMessage = "Test Rule Message", Times.Once);
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
            var result = testableBaseRule.GetConnectableNeighborsWithoutConnection(sourceIslandMock.Object, neighbors);

            // assert
            result.Should().BeEmpty();
        }

        [Test]
        public void AddConnection_ShouldNotThrow_WhenSourceOrTargetIsNull()
        {
            // act & assert
            testableBaseRule.Invoking(x => x.AddConnection(null!, targetIslandMock.Object, connectionManagerMock.Object))
                .Should().NotThrow();

            testableBaseRule.Invoking(x => x.AddConnection(sourceIslandMock.Object, null!, connectionManagerMock.Object))
                .Should().NotThrow();
        }

        [Test]
        public void AddConnection_ShouldNotThrow_WhenConnectionManagerIsNull()
        {
            // act & assert
            testableBaseRule.Invoking(x => x.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, null!))
                .Should().NotThrow();
        }

        [Test]
        public void AddConnection_ShouldNotAddConnection_WhenSourceAndTargetAreSame()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

            sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
            sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);

            targetIslandMock.Setup(x => x.AllConnections).Returns([]);

            // act
            testableBaseRule.AddConnection(sourceIslandMock.Object, sourceIslandMock.Object, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true), Times.Never);
        }

        [Test]
        public void AddConnections_ShouldNotThrow_WhenTargetListIsEmpty()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel?>();
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

            // act & assert
            testableBaseRule.Invoking(x => x.AddConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object))
                .Should().NotThrow();
        }

        [Test]
        public void AddConnections_ShouldHandleDuplicateTargets()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object, targetIslandMock.Object };
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            var hashPointMock = new Mock<IHashiPoint>(MockBehavior.Strict);
            hashPointMock.Setup(mock => mock.X).Returns(1);
            hashPointMock.Setup(mock => mock.Y).Returns(1);

            sourceIslandMock.Setup(mock => mock.Coordinates).Returns(hashPointMock.Object);
            sourceIslandMock.Setup(x => x.AllConnections).Returns([]);
            sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(false);

            targetIslandMock.Setup(x => x.AllConnections).Returns([]);

            // act
            testableBaseRule.AddConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(sourceIslandMock.Object, targetIslandMock.Object, true), Times.Exactly(2));
        }

        [Test]
        public void AddMultipleConnections_ShouldNotThrow_WhenTargetListIsEmpty()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel?>();
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);

            // act & assert
            testableBaseRule.Invoking(x => x.AddMultipleConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object))
                .Should().NotThrow();
        }

        [Test]
        public void AddMultipleConnections_ShouldNotAdd_WhenMaxConnectionsAlreadyExist()
        {
            // arrange
            var targetIslands = new List<IIslandViewModel?> { targetIslandMock.Object };
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            sourceIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);

            // act
            testableBaseRule.AddMultipleConnections(sourceIslandMock.Object, targetIslands, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true), Times.Never);
        }

        [Test]
        public void AddMissingConnectionsToOneTarget_ShouldNotThrow_WhenMissingConnectionsCountIsZero()
        {
            // act & assert
            testableBaseRule.Invoking(x => x.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 0, connectionManagerMock.Object))
                .Should().NotThrow();
        }

        [Test]
        public void AddMissingConnectionsToOneTarget_ShouldNotAdd_WhenTargetHasMaxConnections()
        {
            // arrange
            connectionManagerMock.Setup(cm => cm.AreRulesBeingApplied).Returns(true);
            targetIslandMock.Setup(x => x.MaxConnectionsReached).Returns(true);

            // act
            testableBaseRule.AddMissingConnectionsToOneTarget(sourceIslandMock.Object, targetIslandMock.Object, 2, connectionManagerMock.Object);

            // assert
            connectionManagerMock.Verify(cm => cm.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), true), Times.Never);
        }

        [Test]
        public void GetConnectableNeighborsWithoutConnection_ShouldReturnEmptyList_WhenNoNeighborsExist()
        {
            // arrange
            // act
            var result = testableBaseRule.GetConnectableNeighborsWithoutConnection(sourceIslandMock.Object, new List<IIslandViewModel>());

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
            var result = testableBaseRule.GetConnectedNeighbors(sourceIslandMock.Object, neighbors, 2);

            // assert
            result.Should().BeEmpty();
        }

        [Test]
        public void CountConnectionsToNeighbors_ShouldReturnZero_WhenNoNeighborsExist()
        {
            // act
            var result = testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, new List<IIslandViewModel>());

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
            // act
            var result = testableBaseRule.CountConnectionsToNeighbors(sourceIslandMock.Object, null!);

            // assert
            result.Should().Be(0);
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

            neighborIslandMock1.Setup(x => x.AllConnections).Returns([otherCoordinatesMock.Object]);
            neighborIslandMock2.Setup(x => x.AllConnections).Returns([otherCoordinatesMock.Object]);

            var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

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
            testableBaseRule.Invoking(x => x.ExecuteAddConnection(null!, targetIslandMock.Object, connectionManagerMock.Object))
                .Should().NotThrow();

            testableBaseRule.Invoking(x => x.ExecuteAddConnection(sourceIslandMock.Object, null!, connectionManagerMock.Object))
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
        public void EnsureRulesAreBeingApplied_ShouldNotThrow_WhenConnectionManagerIsNull()
        {
            // act & assert
            testableBaseRule.Invoking(x => x.EnsureRulesAreBeingApplied(null!))
                .Should().NotThrow();
        }
    }

    /// <summary>
    /// A concrete implementation of BaseRule for testing purposes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TestableBaseRule : BaseRule
    {
        /// <summary>
        /// Provides a test-specific rule message.
        /// </summary>
        protected override string RuleMessage => "Test Rule Message";

        /// <summary>
        /// Allows overriding behavior for testing specific scenarios.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public bool EnsureRulesAreBeingAppliedOverride { get; set; } = true;

        /// <summary>
        /// Overrides the EnsureRulesAreBeingApplied method for testing.
        /// </summary>
        [ExcludeFromCodeCoverage]
        internal override bool EnsureRulesAreBeingApplied(IConnectionManagerViewModel? connectionManager)
        {
            return EnsureRulesAreBeingAppliedOverride && base.EnsureRulesAreBeingApplied(connectionManager);
        }

        [ExcludeFromCodeCoverage]
        public override void Define()
        {
            //Do nothing
        }
    }
}
