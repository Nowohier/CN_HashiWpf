using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Extensions;
using Hashi.Rules.Test.Helpers;
using Moq;
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
        public void _5ConnectionsRule3_WhenFourNeighborsWithTwoRestricted_ShouldTriggerRule()
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
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor1.Object, restrictedNeighbor2.Object]);
            testIsland.Setup(mock => mock.CountConnectionsToNeighbors(It.IsAny<List<IIslandViewModel>>()))
                .Returns(3);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([validNeighbor1.Object, validNeighbor2.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor1.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor2.Object, true), Moq.Times.Once);
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
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([]);

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
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor1.Object, restrictedNeighbor2.Object, restrictedNeighbor3.Object]);

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
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor1.Object, restrictedNeighbor2.Object]);

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
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor1.Object, restrictedNeighbor2.Object]);
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
