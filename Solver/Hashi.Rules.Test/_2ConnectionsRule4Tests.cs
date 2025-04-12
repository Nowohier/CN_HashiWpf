using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _2ConnectionsRule4Tests : TestBase
    {
        public _2ConnectionsRule4Tests()
        {
            Setup.Rule<_2ConnectionsRule4>();
        }

        [Test]
        public void _2ConnectionsRule4_WhenTwoNeighborsWithSameMaxConnections_ShouldTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // valid neighbors with the same MaxConnections
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _2ConnectionsRule4_WhenNeighborsHaveDifferentMaxConnections_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // neighbors with different MaxConnections
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule4_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

            // invalid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule4_WhenMoreThanTwoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([]);

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
        public void _2ConnectionsRule4_WhenIslandAlreadyHasConnections_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
