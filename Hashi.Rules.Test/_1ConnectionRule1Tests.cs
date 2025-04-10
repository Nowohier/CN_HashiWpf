using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _1ConnectionRule1Tests : TestBase
    {
        public _1ConnectionRule1Tests()
        {
            Setup.Rule<_1ConnectionRule1>();
        }

        [Test]
        public void _1ConnectionRule1_WhenOneValidNeighbor_ShouldTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 1);

            // valid neighbor
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 2);

            // invalid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2, true);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2, true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Never);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Never);
        }

        [Test]
        public void _1ConnectionRule1_WhenMoreThanOneValidNeighbor_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 1);

            // valid neighbors
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 2);
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);

            // invalid neighbor
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2, true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _1ConnectionRule1_WhenNoValidNeighbor_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 1);

            // invalid neighbors
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 2, true);
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 1);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2, true);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }
    }
}
