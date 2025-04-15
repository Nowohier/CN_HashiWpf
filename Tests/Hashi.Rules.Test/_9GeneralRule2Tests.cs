using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _9GeneralRule2Tests : TestBase<_9GeneralRule2>
{
    // ToDo: Check all tests and add some!
    [Test]
    public void _9GeneralRule2_WhenTwoConnectableNeighborsAndOneIsMaxedOut_ShouldTriggerRule()
    {
        //// arrange
        //var connectableNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, maxConnectionsReached: false);
        //var connectableNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 1, maxConnectionsReached: false);
        //var maxedOutNeighbor = CreateIslandMock(TestIslandEnum.UpIsland, 2, maxConnectionsReached: true);

        //var testIsland = SetupTestIsland(3, connectableNeighbor1, connectableNeighbor2, maxedOutNeighbor);

        //IslandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(1);

        //// act
        //Session.Insert(testIsland.Object);
        //Session.Fire();

        //// assert
        //Verify(x => x.Rule().Fired(Times.Once));
        //IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, connectableNeighbor1.Object, HashiPointTypeEnum.Hint), Moq.Times.Once);
    }

    [Test]
    public void _9GeneralRule2_WhenGroupIsNotIsolated_ShouldNotTriggerRule()
    {
        // arrange
        var connectableNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, maxConnectionsReached: false);
        var connectableNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 1, maxConnectionsReached: false);
        var maxedOutNeighbor = CreateIslandMock(TestIslandEnum.UpIsland, 2, maxConnectionsReached: true);

        var testIsland = SetupTestIsland(3, connectableNeighbor1, connectableNeighbor2, maxedOutNeighbor);

        IslandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(0);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _9GeneralRule2_WhenMoreThanTwoConnectableNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        var connectableNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, maxConnectionsReached: false);
        var connectableNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 1, maxConnectionsReached: false);
        var connectableNeighbor3 = CreateIslandMock(TestIslandEnum.DownIsland, 1, maxConnectionsReached: false);

        var testIsland = SetupTestIsland(3, connectableNeighbor1, connectableNeighbor2, connectableNeighbor3);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _9GeneralRule2_WhenNoConnectableNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        var maxedOutNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, maxConnectionsReached: true);
        var maxedOutNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 1, maxConnectionsReached: true);

        var testIsland = SetupTestIsland(3, maxedOutNeighbor1, maxedOutNeighbor2);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _9GeneralRule2_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
    {
        // arrange
        var connectableNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 1, maxConnectionsReached: false);

        var testIsland = SetupTestIsland(3, connectableNeighbor);
        testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}
