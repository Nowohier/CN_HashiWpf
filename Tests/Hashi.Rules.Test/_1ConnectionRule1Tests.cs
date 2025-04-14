using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _1ConnectionRule1Tests : TestBase<_1ConnectionRule1>
{
    [Test]
    public void _1ConnectionRule1_WhenOneValidNeighbor_ShouldTriggerRule()
    {
        // arrange
        // valid neighbor
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 2);

        // invalid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2, true);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2, true);

        var testIsland = SetupTestIsland(1, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true),
            Moq.Times.Once);
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true),
            Moq.Times.Never);
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true),
            Moq.Times.Never);
    }

    [Test]
    public void _1ConnectionRule1_WhenMoreThanOneValidNeighbor_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 2);
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);

        // invalid neighbor
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2, true);

        var testIsland = SetupTestIsland(1, leftIsland, rightIsland, upIsland);

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
        // invalid neighbors
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 2, true);
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2, true);
        var testIsland = SetupTestIsland(1, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}