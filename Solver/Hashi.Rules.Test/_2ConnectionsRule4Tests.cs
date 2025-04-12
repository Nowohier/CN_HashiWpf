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
        [TestCase(3, 3, true)] // Same MaxConnections, should trigger
        [TestCase(3, 2, false)] // Different MaxConnections, should not trigger
        public void _2ConnectionsRule4_ShouldTriggerBasedOnNeighborMaxConnections(int leftMaxConnections, int rightMaxConnections, bool shouldTrigger)
        {
            // arrange
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, leftMaxConnections);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, rightMaxConnections);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestee(2, leftIsland, rightIsland);

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
        public void _2ConnectionsRule4_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // invalid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
            var testIsland = SetupTestee(2, leftIsland, rightIsland);

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
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            var testIsland = SetupTestee(2, leftIsland, rightIsland, upIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule4_WhenMixedValidAndInvalidNeighbors_ShouldTriggerRule()
        {
            // arrange
            var validNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var invalidNeighbor = CreateIslandMock(TestIslandEnum.RightIsland, 2);
            invalidNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestee(2, validNeighbor, invalidNeighbor);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _2ConnectionsRule4_WhenIslandAlreadyHasConnections_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var testIsland = SetupTestee(2, leftIsland, rightIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
