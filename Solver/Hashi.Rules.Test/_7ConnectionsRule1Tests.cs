using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Extensions;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _7ConnectionsRule1Tests : TestBase
    {
        public _7ConnectionsRule1Tests()
        {
            Setup.Rule<_7ConnectionsRule1>();
        }

        [Test]
        public void _7ConnectionsRule1_WhenFourValidNeighborsAndNotAllConnected_ShouldTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 7);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);
            testIsland.Setup(mock => mock.AreAllNeighborsConnected(It.IsAny<List<IIslandViewModel>>())).Returns(false);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            var downIsland = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object, downIsland.Object]);

            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object, downIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, downIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _7ConnectionsRule1_WhenAllNeighborsAreConnected_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 7);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);
            testIsland.Setup(mock => mock.AreAllNeighborsConnected(It.IsAny<List<IIslandViewModel>>())).Returns(true);

            // neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            var downIsland = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object, downIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _7ConnectionsRule1_WhenLessThanFourNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 7);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);
            testIsland.Setup(mock => mock.AreAllNeighborsConnected(It.IsAny<List<IIslandViewModel>>())).Returns(false);

            // neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _7ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 7);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            var downIsland = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object, downIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _7ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 7);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);
            testIsland.Setup(mock => mock.AreAllNeighborsConnected(It.IsAny<List<IIslandViewModel>>())).Returns(false);

            // neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            var downIsland = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object, downIsland.Object]);

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
