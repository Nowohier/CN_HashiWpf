using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _8ConnectionsRule1Tests : TestBase
{
    public _8ConnectionsRule1Tests()
    {
        Setup.Rule<_8ConnectionsRule1>();
    }

    [Test]
    public void _8ConnectionsRule1_WhenFourValidNeighbors_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        var downIsland = CreateIslandMock(TestIslandEnum.DownIsland, 3);

        var testIsland = SetupTestIsland(8, leftIsland, rightIsland, upIsland, downIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true),
            Moq.Times.Exactly(2));
        ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, true),
            Moq.Times.Exactly(2));
        ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true),
            Moq.Times.Exactly(2));
        ConnectionManagerMock.Verify(mock => mock.AddConnection(testIsland.Object, downIsland.Object, true),
            Moq.Times.Exactly(2));
    }

    [Test]
    public void _8ConnectionsRule1_WhenLessThanFourNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(8, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _8ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        var downIsland = CreateIslandMock(TestIslandEnum.DownIsland, 3);

        var testIsland = SetupTestIsland(8, leftIsland, rightIsland, upIsland, downIsland);
        testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _8ConnectionsRule1_WhenNoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        var testIsland = SetupTestIsland(8);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}