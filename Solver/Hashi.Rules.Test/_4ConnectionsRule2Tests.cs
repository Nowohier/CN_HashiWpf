using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Extensions;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _4ConnectionsRule2Tests : TestBase
    {
        public _4ConnectionsRule2Tests()
        {
            Setup.Rule<_4ConnectionsRule2>();
        }

        [Test]
        public void _4ConnectionsRule2_WhenThreeNeighborsWithOneRestrictedNeighbor_ShouldTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, restrictedNeighbor, validNeighbor1, validNeighbor2);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor.Object]);
            testIsland.Setup(mock => mock.CountConnectionsToNeighbors(It.IsAny<List<IIslandViewModel>>()))
                .Returns(1);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([validNeighbor1.Object, validNeighbor2.Object]);

            restrictedNeighbor.Setup(mock => mock.AllConnections).Returns([testIsland.Object.Coordinates]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor1.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor2.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _4ConnectionsRule2_WhenNoRestrictedNeighbor_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor3 = CreateIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, validNeighbor1, validNeighbor2, validNeighbor3);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule2_WhenMoreThanOneRestrictedNeighbor_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, restrictedNeighbor1, restrictedNeighbor2, validNeighbor);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor1.Object, restrictedNeighbor2.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule2_WhenIslandHasInvalidRemainingConnections_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, restrictedNeighbor, validNeighbor1, validNeighbor2);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(false);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule2_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            // neighbors
            var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var invalidNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            invalidNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var invalidNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            invalidNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestIsland(4, restrictedNeighbor, invalidNeighbor1, invalidNeighbor2);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor.Object]);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule2_WhenNoNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = SetupTestIsland(4);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule2_WhenAllNeighborsHaveMaxConnections_ShouldNotTriggerRule()
        {
            // arrange
            var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            restrictedNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestIsland(4, restrictedNeighbor1, restrictedNeighbor2, restrictedNeighbor3);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor1.Object, restrictedNeighbor2.Object, restrictedNeighbor3.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _4ConnectionsRule2_WhenMixedNeighbors_ShouldTriggerRule()
        {
            // arrange
            var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var invalidNeighbor = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            invalidNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var testIsland = SetupTestIsland(4, restrictedNeighbor, validNeighbor1, invalidNeighbor);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(true);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor.Object]);
            testIsland.Setup(mock => mock.GetConnectableNeighborsWithoutConnection(It.IsAny<List<IIslandViewModel>>()))
                .Returns([validNeighbor1.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor1.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _4ConnectionsRule2_WhenInvalidRemainingConnections_ShouldNotTriggerRule()
        {
            // arrange
            var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var testIsland = SetupTestIsland(4, restrictedNeighbor, validNeighbor1, validNeighbor2);
            testIsland.Setup(mock => mock.AreRemainingConnectionsWithinRange(2, 3)).Returns(false);
            testIsland.Setup(mock => mock.GetMaxedOutConnectedNeighbors(It.IsAny<List<IIslandViewModel>>(), null))
                .Returns([restrictedNeighbor.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

    }
}
