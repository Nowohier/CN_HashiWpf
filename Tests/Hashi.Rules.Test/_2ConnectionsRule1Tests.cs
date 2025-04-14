using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
[NonParallelizable]
public class _2ConnectionsRule1Tests : TestBase<_2ConnectionsRule1>
{
    [Test]
    public void _2ConnectionsRule1_WhenTwoValidNeighbors_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        var testIsland = SetupTestIsland(2, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true),
            Moq.Times.Once);
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true),
            Moq.Times.Once);
    }

    [Test]
    public void
        _2ConnectionsRule1_WhenTwoValidNeighborsAndAdditionalNeighbors_ShouldNotTriggerRule() //ToDo: This fails when running whole class, but not when running this test alone
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);

        // invalid neighbor
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 4);

        var testIsland = SetupTestIsland(2, leftIsland, rightIsland, upIsland);

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
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 2);
        var testIsland = SetupTestIsland(2, leftIsland, rightIsland, upIsland);

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
        // valid neighbor
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);

        // invalid neighbor
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 1);

        var testIsland = SetupTestIsland(2, leftIsland, upIsland);

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
        // invalid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 1);
        var testIsland = SetupTestIsland(2, leftIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}