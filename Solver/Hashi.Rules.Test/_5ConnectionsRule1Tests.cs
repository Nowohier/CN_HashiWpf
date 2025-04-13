using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Extensions;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _5ConnectionsRule1Tests : TestBase
    {
        public _5ConnectionsRule1Tests()
        {
            Setup.Rule<_5ConnectionsRule1>();
        }

        [Test]
        public void _5ConnectionsRule1_WhenThreeValidNeighbors_ShouldTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

            var testIsland = SetupTestIsland(5, leftIsland, rightIsland, upIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _5ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // invalid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            upIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestIsland(5, leftIsland, rightIsland, upIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _5ConnectionsRule1_WhenLessThanThreeNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

            var testIsland = SetupTestIsland(5, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _5ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

            var testIsland = SetupTestIsland(5, leftIsland, rightIsland, upIsland);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _5ConnectionsRule1_WhenNoConnectableNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

            var testIsland = SetupTestIsland(5, leftIsland, rightIsland, upIsland);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
