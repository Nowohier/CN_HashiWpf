using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Extensions;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _3ConnectionsRule1Tests : TestBase
    {
        public _3ConnectionsRule1Tests()
        {
            Setup.Rule<_3ConnectionsRule1>();
        }

        [Test]
        public void _3ConnectionsRule1_WhenTwoValidNeighbors_ShouldTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestee(3, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _3ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // invalid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestee(3, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _3ConnectionsRule1_WhenMoreThanTwoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

            var testIsland = SetupTestee(3, leftIsland, rightIsland, upIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _3ConnectionsRule1_WhenIslandAlreadyHasTwoConnections_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

            var testIsland = SetupTestee(3, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.AllConnections).Returns([
                CreateHashiPointMock(0, 1).Object,
                CreateHashiPointMock(1, 0).Object
            ]);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _3ConnectionsRule1_WhenIslandHasLessThanTwoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbor
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);

            var testIsland = SetupTestee(3, leftIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _3ConnectionsRule1_WhenNoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = SetupTestee(3);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _3ConnectionsRule1_WhenAllNeighborsHaveMaxConnections_ShouldNotTriggerRule()
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestee(3, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _3ConnectionsRule1_WhenMixedNeighbors_ShouldTriggerRule()
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestee(3, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _3ConnectionsRule1_WhenInvalidRemainingConnections_ShouldNotTriggerRule()
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestee(3, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(false);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

    }
}
