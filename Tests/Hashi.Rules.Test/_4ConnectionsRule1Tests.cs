using Hashi.Enums;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _4ConnectionsRule1Tests : TestBase<_4ConnectionsRule1>
{
    [Test]
    public void _4ConnectionsRule1_WhenTwoValidNeighbors_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

        var testIsland = SetupTestIsland(4, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, rightIsland.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
    }

    [Test]
    public void _4ConnectionsRule1_WhenNoValidNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // invalid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var testIsland = SetupTestIsland(4, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _4ConnectionsRule1_WhenMoreThanTwoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(4, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _4ConnectionsRule1_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

        var testIsland = SetupTestIsland(4, leftIsland, rightIsland);
        testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _4ConnectionsRule1_WhenIslandHasLessThanTwoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbor
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);

        var testIsland = SetupTestIsland(4, leftIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}