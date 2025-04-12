using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _8ConnectionsRule1Tests : TestBase
    {
        public _8ConnectionsRule1Tests()
        {
            Setup.Rule<_8ConnectionsRule1>();
        }

        [Test]
        public void _8ConnectionsRule1_WhenFourValidNeighbors_ShouldTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 8);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // valid neighbors
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
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, downIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _8ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 8);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // invalid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            upIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var downIsland = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);
            downIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object, downIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _8ConnectionsRule1_WhenLessThanFourNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 8);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // valid neighbors
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
        public void _8ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 8);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // valid neighbors
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
        public void _8ConnectionsRule1_WhenNoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 8);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}

