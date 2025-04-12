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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 5);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // neighbors
            var restrictedNeighbor1 = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([
                    restrictedNeighbor1.Object, restrictedNeighbor2.Object, validNeighbor1.Object, validNeighbor2.Object
                ]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 5);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // neighbors
            var validNeighbor1 = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor3 = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor4 = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            validNeighbor4.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([validNeighbor1.Object, validNeighbor2.Object, validNeighbor3.Object, validNeighbor4.Object]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 5);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // neighbors
            var restrictedNeighbor1 = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor3 = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            restrictedNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([
                    restrictedNeighbor1.Object, restrictedNeighbor2.Object, restrictedNeighbor3.Object,
                    validNeighbor.Object
                ]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 5);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            // neighbors
            var restrictedNeighbor1 = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var validNeighbor1 = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            var validNeighbor2 = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);
            validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([
                    restrictedNeighbor1.Object, restrictedNeighbor2.Object, validNeighbor1.Object, validNeighbor2.Object
                ]);

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
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 5);
            testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

            // neighbors
            var restrictedNeighbor1 = CreateTestIslandMock(TestIslandEnum.LeftIsland, 3);
            restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var restrictedNeighbor2 = CreateTestIslandMock(TestIslandEnum.RightIsland, 3);
            restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var invalidNeighbor1 = CreateTestIslandMock(TestIslandEnum.UpIsland, 3);
            invalidNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            var invalidNeighbor2 = CreateTestIslandMock(TestIslandEnum.DownIsland, 3);
            invalidNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([
                    restrictedNeighbor1.Object, restrictedNeighbor2.Object, invalidNeighbor1.Object,
                    invalidNeighbor2.Object
                ]);

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
