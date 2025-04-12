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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 4);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // valid neighbor
            var validNeighbor = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([validNeighbor.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 4);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // invalid neighbor
            var invalidNeighbor = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            invalidNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([invalidNeighbor.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 4);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // valid neighbors
            var validNeighbor1 = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([validNeighbor1.Object, validNeighbor2.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 4);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // valid neighbor
            var validNeighbor = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([validNeighbor.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 4);
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

