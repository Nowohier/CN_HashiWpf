using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test
{
    [TestFixture]
    public class _2ConnectionsRule1Tests : TestBase
    {
        public _2ConnectionsRule1Tests()
        {
            Setup.Rule<_2ConnectionsRule1>();
        }

        [Test]
        public void _2ConnectionsRule1_WhenTwoValidNeighbors_ShouldTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Once));
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true), Moq.Times.Once);
            ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true), Moq.Times.Once);
        }

        [Test]
        public void _2ConnectionsRule1_WhenTwoValidNeighborsAndAdditionalNeighbors_ShouldNotTriggerRule() //ToDo: This fails when running whole class, but not when running this test alone - race condition? -> Fix
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2);

            // invalid neighbor
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 4);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule1_WhenMoreThanTwoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);

            // valid neighbors
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);
            var rightIsland = CreateTestIslandMock(TestIslandEnum.RightIsland, 2);
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 2);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule1_WhenLessThanTwoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);

            // valid neighbor
            var leftIsland = CreateTestIslandMock(TestIslandEnum.LeftIsland, 2);

            // invalid neighbor
            var upIsland = CreateTestIslandMock(TestIslandEnum.UpIsland, 1);

            testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
                .Returns([leftIsland.Object, upIsland.Object]);

            // act
            Session.Insert(testIsland.Object);
            Session.Fire();

            // assert
            Verify(x => x.Rule().Fired(Times.Never));
        }

        [Test]
        public void _2ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
        {
            // arrange
            var testIsland = CreateTestIslandMock(TestIslandEnum.TestIsland, 2);

            // invalid neighbors
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
