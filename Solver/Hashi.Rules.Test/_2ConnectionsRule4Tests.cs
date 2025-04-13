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
        [TestCase(3, 1, true)]
        [TestCase(3, 2, false)]
        public void _2ConnectionsRule4_WhenTwoNeighbors_ShouldTriggerBasedOnNeighborMaxConnections(int leftMaxConnections, int rightMaxConnections, bool shouldTrigger)
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, leftMaxConnections);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, rightMaxConnections);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(2, leftIsland, rightIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            if (shouldTrigger)
            {
                Verify(x => x.Rule().Fired(Times.Once));
                ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            }
            else
            {
                Verify(x => x.Rule().Fired(Times.Never));
            }
        }

        [Test]
        [TestCase(3, 1, 1, true)]
        [TestCase(3, 2, 1, false)]
        public void _2ConnectionsRule4_WhenThreeNeighbors_ShouldTriggerBasedOnNeighborMaxConnections(int leftMaxConnections, int rightMaxConnections, int upMaxConnections, bool shouldTrigger)
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, leftMaxConnections);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, rightMaxConnections);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var upIsland = CreateIslandMock(TestIslandEnum.RightIsland, upMaxConnections);
            upIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(2, leftIsland, rightIsland, upIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            if (shouldTrigger)
            {
                Verify(x => x.Rule().Fired(Times.Once));
                ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            }
            else
            {
                Verify(x => x.Rule().Fired(Times.Never));
            }
        }

        [Test]
        [TestCase(3, 1, 1, 1, true)]
        [TestCase(3, 2, 1, 1, false)]
        [TestCase(3, 2, 2, 1, false)]
        public void _2ConnectionsRule4_WhenFourNeighbors_ShouldTriggerBasedOnNeighborMaxConnections(int leftMaxConnections, int rightMaxConnections, int upMaxConnections, int downMaxConnections, bool shouldTrigger)
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, leftMaxConnections);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, rightMaxConnections);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var upIsland = CreateIslandMock(TestIslandEnum.RightIsland, upMaxConnections);
            upIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var downIsland = CreateIslandMock(TestIslandEnum.RightIsland, downMaxConnections);
            downIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(2, leftIsland, rightIsland, upIsland, downIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            if (shouldTrigger)
            {
                Verify(x => x.Rule().Fired(Times.Once));
                ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            }
            else
            {
                Verify(x => x.Rule().Fired(Times.Never));
            }
        }

        [Test]
        public void _2ConnectionsRule4_WhenMoreThanOneNeighborsBiggerMaxConnectionsOne_ShouldNotTriggerRule()
        {
            // arrange
            // invalid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
            var testIsland = SetupTestIsland(2, leftIsland, rightIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
