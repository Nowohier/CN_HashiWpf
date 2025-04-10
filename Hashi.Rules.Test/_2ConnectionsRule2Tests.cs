using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _2ConnectionsRule2Tests : TestBase
    {
        public _2ConnectionsRule2Tests()
        {
            Setup.Rule<_2ConnectionsRule2>();
        }

        [Test]
        public void _2ConnectionsRule2_WhenOneNeighborWithMaxConnectionsOneAndOneValidNeighbor_ShouldTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

            // valid neighbor
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);

            // neighbor with MaxConnections == 1 and already connected
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 1);
            upIsland.Setup(mock => mock.AllConnections).Returns([testIsland.Object.Coordinates]);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
        }

        [Test]
        public void _2ConnectionsRule2_WhenNoNeighborWithMaxConnectionsOne_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 2);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule2_WhenMoreThanOneValidNeighbor_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2);

            // neighbor with MaxConnections == 1 and already connected
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 1);
            upIsland.Setup(mock => mock.AllConnections).Returns([testIsland.Object.Coordinates]);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule2_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);
            testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

            // neighbors with MaxConnections == 1 but no valid connections
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 1);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 1);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
