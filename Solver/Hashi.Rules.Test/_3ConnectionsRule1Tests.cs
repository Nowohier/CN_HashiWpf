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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 3);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 3);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // invalid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 3);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 3);
            testIsland.Setup(mock => mock.AllConnections).Returns([
                CreateHashiPointMock(0, 1).Object,
                CreateHashiPointMock(1, 0).Object
            ]);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 3);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // valid neighbor
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object]);

            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
