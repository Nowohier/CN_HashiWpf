using Hashi.Enums;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _5ConnectionsRule2Tests : TestBase<_5ConnectionsRule2>
{
    [Test]
    public void _5ConnectionsRule2_WhenFourNeighborsWithOneConnectedRestrictedNeighbor_ShouldTriggerRule()
    {
        // arrange
        // neighbors
        var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3, true);
        var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.DownIsland, 1, true);
        restrictedNeighbor2.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(1, 1).Object]);
        var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(5, restrictedNeighbor1, restrictedNeighbor2, validNeighbor1, validNeighbor2);
        testIsland.Setup(mock => mock.AllConnections).Returns([CreateHashiPointMock(1, 1).Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor1.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, validNeighbor2.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Exactly(2));
    }

    [Test]
    public void _5ConnectionsRule2_WhenNoRestrictedNeighbor_ShouldNotTriggerRule()
    {
        // arrange
        // neighbors
        var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var validNeighbor3 = CreateIslandMock(TestIslandEnum.DownIsland, 3);
        validNeighbor3.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var testIsland = SetupTestIsland(5, validNeighbor1, validNeighbor2, validNeighbor3);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _5ConnectionsRule2_WhenMoreThanOneRestrictedNeighbor_ShouldNotTriggerRule()
    {
        // arrange
        // neighbors
        var restrictedNeighbor1 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        restrictedNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var restrictedNeighbor2 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        restrictedNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var validNeighbor = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        validNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var testIsland = SetupTestIsland(5, restrictedNeighbor1, restrictedNeighbor2, validNeighbor);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _5ConnectionsRule2_WhenIslandHasMaxConnectionsReached_ShouldNotTriggerRule()
    {
        // arrange
        // neighbors
        var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var validNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        validNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var validNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        validNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(false);

        var testIsland = SetupTestIsland(5, restrictedNeighbor, validNeighbor1, validNeighbor2);
        testIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _5ConnectionsRule2_WhenNoValidNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // neighbors
        var restrictedNeighbor = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        restrictedNeighbor.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var invalidNeighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        invalidNeighbor1.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var invalidNeighbor2 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        invalidNeighbor2.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var testIsland = SetupTestIsland(5, restrictedNeighbor, invalidNeighbor1, invalidNeighbor2);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}