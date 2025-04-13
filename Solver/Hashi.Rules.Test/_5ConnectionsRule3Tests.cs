using Hashi.Gui.Interfaces.Models;
using Hashi.Rules.Test.Helpers;
using System.Collections.ObjectModel;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _5ConnectionsRule3Tests : TestBase
    {
        public _5ConnectionsRule3Tests()
        {
            Setup.Rule<_5ConnectionsRule3>();
        }

        [Test]
        [TestCase(2, 1, true)]
        [TestCase(1, 1, true)]
        [TestCase(0, 1, false)]
        [TestCase(2, 2, false)]
        public void _5ConnectionsRule3_WhenFourNeighborsWithTwoRestricted_ShouldTriggerRule(int amountRestrictedConnectionsToSource1, int amountRestrictedConnectionsToSource2, bool ruleIsFired)
        {
            // arrange
            var hashiPointMocks = new List<IHashiPoint>([CreateHashiPointMock(1, 1).Object, CreateHashiPointMock(1, 1).Object]);

            // neighbors
            var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);
            restrictedNeighbor1.Setup(mock => mock.AllConnections).Returns(new ObservableCollection<IHashiPoint>(hashiPointMocks.Take(amountRestrictedConnectionsToSource1).ToList()));

            var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);
            restrictedNeighbor2.Setup(mock => mock.AllConnections).Returns(new ObservableCollection<IHashiPoint>(hashiPointMocks.Take(amountRestrictedConnectionsToSource2).ToList()));

            var validNeighbor1 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(5, restrictedNeighbor1, restrictedNeighbor2, validNeighbor1, validNeighbor2);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(ruleIsFired ? Times.Once : Times.Never));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor1.Object, true), ruleIsFired ? Moq.Times.Once : Moq.Times.Never);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor2.Object, true), ruleIsFired ? Moq.Times.Once : Moq.Times.Never);
        }

        [Test]
        public void _5ConnectionsRule3_WhenNoRestrictedNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor3 = CreateIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor4 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor4.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(5, validNeighbor1, validNeighbor2, validNeighbor3, validNeighbor4);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _5ConnectionsRule3_WhenMoreThanTwoRestrictedNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            restrictedNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor = CreateIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(5, restrictedNeighbor1, restrictedNeighbor2, restrictedNeighbor3, validNeighbor);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _5ConnectionsRule3_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(5, restrictedNeighbor1, restrictedNeighbor2, validNeighbor1, validNeighbor2);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _5ConnectionsRule3_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var invalidNeighbor1 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            invalidNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var invalidNeighbor2 = CreateIslandMock(TestIslandEnum.DownIsland, 3);
            invalidNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestIsland(5, restrictedNeighbor1, restrictedNeighbor2, invalidNeighbor1, invalidNeighbor2);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
