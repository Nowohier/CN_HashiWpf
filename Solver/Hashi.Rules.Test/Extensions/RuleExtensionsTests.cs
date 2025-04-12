using FluentAssertions;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Extensions;
using Moq;
using System.Collections.ObjectModel;

namespace Hashi.Rules.Test.Extensions
{
    [TestFixture]
    public class RuleExtensionsTests
    {
        private Mock<IIslandViewModel> sourceIslandMock;
        private Mock<IIslandViewModel> neighborIslandMock1;
        private Mock<IIslandViewModel> neighborIslandMock2;
        private Mock<IHashiPoint> sourceCoordinatesMock;

        [SetUp]
        public void SetUp()
        {
            sourceIslandMock = new Mock<IIslandViewModel>();
            neighborIslandMock1 = new Mock<IIslandViewModel>();
            neighborIslandMock2 = new Mock<IIslandViewModel>();
            sourceCoordinatesMock = new Mock<IHashiPoint>();

            sourceIslandMock.Setup(x => x.Coordinates).Returns(sourceCoordinatesMock.Object);
        }

        [Test]
        public void GetConnectableNeighborsWithoutConnection_ShouldReturnOnlyConnectableNeighbors()
        {
            // arrange
            neighborIslandMock1.Setup(x => x.MaxConnectionsReached).Returns(false);
            neighborIslandMock1.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            neighborIslandMock2.Setup(x => x.MaxConnectionsReached).Returns(true);
            neighborIslandMock2.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

            // act
            var result = sourceIslandMock.Object.GetConnectableNeighborsWithoutConnection(neighbors);

            // assert
            result.Should().HaveCount(1);
            result.Should().Contain(neighborIslandMock1.Object);
        }

        [Test]
        public void AreAllNeighborsConnected_ShouldReturnTrue_WhenAllNeighborsAreConnected()
        {
            // arrange
            neighborIslandMock1.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object });
            neighborIslandMock2.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object });

            var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

            // act
            var result = sourceIslandMock.Object.AreAllNeighborsConnected(neighbors);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void AreAllNeighborsConnected_ShouldReturnFalse_WhenNotAllNeighborsAreConnected()
        {
            // arrange
            neighborIslandMock1.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object });
            neighborIslandMock2.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint>());

            var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

            // act
            var result = sourceIslandMock.Object.AreAllNeighborsConnected(neighbors);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void CountConnectionsToNeighbors_ShouldReturnCorrectCount()
        {
            // arrange
            neighborIslandMock1.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object, sourceCoordinatesMock.Object });
            neighborIslandMock2.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object });

            var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

            // act
            var result = sourceIslandMock.Object.CountConnectionsToNeighbors(neighbors);

            // assert
            result.Should().Be(3);
        }

        [Test]
        public void AreRemainingConnectionsWithinRange_ShouldReturnTrue_WhenWithinRange()
        {
            // arrange
            sourceIslandMock.Setup(x => x.RemainingConnections).Returns(3);

            // act
            var result = sourceIslandMock.Object.AreRemainingConnectionsWithinRange(2, 4);

            // assert
            result.Should().BeTrue();
        }

        [Test]
        public void AreRemainingConnectionsWithinRange_ShouldReturnFalse_WhenOutOfRange()
        {
            // arrange
            sourceIslandMock.Setup(x => x.RemainingConnections).Returns(5);

            // act
            var result = sourceIslandMock.Object.AreRemainingConnectionsWithinRange(2, 4);

            // assert
            result.Should().BeFalse();
        }

        [Test]
        public void GetMaxedOutConnectedNeighbors_ShouldReturnOnlyMaxedOutNeighbors()
        {
            // arrange
            neighborIslandMock1.Setup(x => x.MaxConnectionsReached).Returns(true);
            neighborIslandMock1.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object });

            neighborIslandMock2.Setup(x => x.MaxConnectionsReached).Returns(false);
            neighborIslandMock2.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoordinatesMock.Object });

            var neighbors = new List<IIslandViewModel> { neighborIslandMock1.Object, neighborIslandMock2.Object };

            // act
            var result = sourceIslandMock.Object.GetMaxedOutConnectedNeighbors(neighbors, null);

            // assert
            result.Should().HaveCount(1);
            result.Should().Contain(neighborIslandMock1.Object);
        }
    }
}
