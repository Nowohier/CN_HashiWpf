using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _4ConnectionsRule1Tests : TestBase
    {
        public _4ConnectionsRule1Tests()
        {
            Setup.Rule<_4ConnectionsRule1>();
        }

        [Test]
        public void _4ConnectionsRule1_WhenTwoValidNeighbors_ShouldTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

            var testIsland = SetupTestee(4, leftIsland, rightIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _4ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // invalid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestee(4, leftIsland, rightIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule1_WhenMoreThanTwoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

            var testIsland = SetupTestee(4, leftIsland, rightIsland, upIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

            var testIsland = SetupTestee(4, leftIsland, rightIsland);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule1_WhenIslandHasLessThanTwoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbor
            var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);

            var testIsland = SetupTestee(4, leftIsland);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
