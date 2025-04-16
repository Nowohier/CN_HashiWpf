using Hashi.Enums;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _6ConnectionsRule1Tests : TestBase<_6ConnectionsRule1>
{
    [Test]
    public void _6ConnectionsRule1_WhenThreeValidNeighbors_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(6, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
    }

    [Test]
    public void _6ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // invalid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        upIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var testIsland = SetupTestIsland(6, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _6ConnectionsRule1_WhenLessThanThreeNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

        var testIsland = SetupTestIsland(6, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _6ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(6, leftIsland, rightIsland, upIsland);
        testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _6ConnectionsRule1_WhenNoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        var testIsland = SetupTestIsland(6);
        IslandProviderMock.Setup(mock => mock.GetAllVisibleNeighbors(testIsland.Object))
            .Returns([]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}