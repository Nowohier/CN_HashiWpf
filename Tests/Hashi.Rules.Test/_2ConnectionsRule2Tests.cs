using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _2ConnectionsRule2Tests : TestBase
{
    public _2ConnectionsRule2Tests()
    {
        Setup.Rule<_2ConnectionsRule2>();
    }

    [Test]
    public void _2ConnectionsRule2_WhenOneNeighborWithMaxConnectionsOneAndOneValidNeighbor_ShouldTriggerRule()
    {
        // arrange
        // valid neighbor
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 1);

        var testIsland = SetupTestIsland(2, leftIsland, upIsland);
        testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

        // neighbor with MaxConnections == 1 and already connected
        upIsland.Setup(mock => mock.AllConnections).Returns([testIsland.Object.Coordinates]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, leftIsland.Object, true),
            Moq.Times.Once);
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, upIsland.Object, true),
            Moq.Times.Never);
    }

    [Test]
    public void _2ConnectionsRule2_WhenNoNeighborWithMaxConnectionsOne_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 2);

        var testIsland = SetupTestIsland(2, leftIsland, upIsland);
        testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _2ConnectionsRule2_WhenMoreThanOneValidNeighbor_ShouldNotTriggerRule()
    {
        // arrange
        var testIsland = CreateIslandMock(TestIslandEnum.TestIsland, 2);
        testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);

        // neighbor with MaxConnections == 1 and already connected
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 1);
        upIsland.Setup(mock => mock.AllConnections).Returns([testIsland.Object.Coordinates]);

        testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
            .Returns([leftIsland.Object, rightIsland.Object, upIsland.Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _2ConnectionsRule2_WhenNoValidNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        var testIsland = CreateIslandMock(TestIslandEnum.TestIsland, 2);
        testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);

        // neighbors with MaxConnections == 1 but no valid connections
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 1);

        testIsland.Setup(mock => mock.GetAllVisibleNeighbors())
            .Returns([leftIsland.Object, upIsland.Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}