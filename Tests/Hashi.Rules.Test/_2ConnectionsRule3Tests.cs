using Hashi.Enums;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _2ConnectionsRule3Tests : TestBase<_2ConnectionsRule3>
{
    [Test]
    public void _2ConnectionsRule3_WhenTwoNeighborsWithValidConditions_ShouldTriggerRule()
    {
        // arrange
        // valid neighbor
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        // invalid neighbor
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var testIsland = SetupTestIsland(2, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(
            mock => mock.AddConnection(testIsland.Object, leftIsland.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Once);
    }

    [Test]
    public void _2ConnectionsRule3_WhenNoValidNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // invalid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);

        var testIsland = SetupTestIsland(2, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _2ConnectionsRule3_WhenMoreThanTwoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(2, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _2ConnectionsRule3_WhenOneNeighborHasMaxConnectionsReached_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbor
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        // invalid neighbor with MaxConnectionsReached
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var testIsland = SetupTestIsland(2, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _2ConnectionsRule3_WhenIslandAlreadyHasConnections_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);

        var testIsland = SetupTestIsland(2, leftIsland, rightIsland);
        testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}