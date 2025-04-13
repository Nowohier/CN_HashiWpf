using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _9GeneralRule1Tests : TestBase
    {
        public _9GeneralRule1Tests()
        {
            Setup.Rule<_9GeneralRule1>();
        }

        [Test]
        public void _9GeneralRule1_WhenOneValidNeighbor_ShouldTriggerRule()
        {
            // arrange
            // valid neighbor
            var validNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, validNeighbor);
            testIsland.Setup(mock => mock.AllConnections.Count).Returns(1);

            var missingConnections = testIsland.Object.MaxConnections - testIsland.Object.AllConnections.Count;

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor.Object, true), Moq.Times.Exactly(missingConnections));
        }

        [Test]
        public void _9GeneralRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // invalid neighbor
            var invalidNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            invalidNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestIsland(4, invalidNeighbor);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _9GeneralRule1_WhenMoreThanOneValidNeighbor_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbors
            var validNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, validNeighbor1, validNeighbor2);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _9GeneralRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
        {
            // arrange
            // valid neighbor
            var validNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, validNeighbor);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _9GeneralRule1_WhenNoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = SetupTestIsland(4);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}

