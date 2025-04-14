using Hashi.Enums;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

[TestFixture]
public class _3ConnectionsRule2Tests : TestBase<_3ConnectionsRule2>
{
    [Test]
    public void _3ConnectionsRule2_WhenThreeValidNeighbors_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        var upIslandMock = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIslandMock);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, upIslandMock.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Once);
    }

    [Test]
    public void _3ConnectionsRule2_WhenThreeValidNeighborsAndTwoConnectionsSet_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        rightIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(2, 1).Object, CreateHashiPointMock(2, 1).Object]);
        var upIslandMock = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIslandMock);
        testIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(2, 1).Object, CreateHashiPointMock(2, 1).Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, upIslandMock.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Once);
    }

    [Test]
    public void _3ConnectionsRule2_WhenThreeValidNeighborsAndTwoConnectionsSet2_ShouldTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        leftIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        rightIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(2, 1).Object]);
        var upIslandMock = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIslandMock);
        testIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(0, 1).Object, CreateHashiPointMock(2, 1).Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Once));
        IslandProviderMock.Verify(mock => mock.AddConnection(testIsland.Object, upIslandMock.Object, HashiPointTypeEnum.Hint),
            Moq.Times.Once);
    }

    [Test]
    public void _3ConnectionsRule2_WhenMoreThanThreeNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        var downIsland = CreateIslandMock(TestIslandEnum.DownIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIsland, downIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _3ConnectionsRule2_WhenIslandWithMaxOneConnectionIsMissing_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _3ConnectionsRule2_WhenIslandWithMaxTwoConnectionsIsMissing_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var upIsland = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _3ConnectionsRule2_WhenTestIslandAlreadyHasThreeConnections_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbors
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        leftIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(0, 1).Object]);
        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 2);
        rightIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(2, 1).Object, CreateHashiPointMock(2, 1).Object]);
        var upIslandMock = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland, upIslandMock);
        testIsland.Setup(x => x.AllConnections).Returns([CreateHashiPointMock(2, 1).Object, CreateHashiPointMock(2, 1).Object, CreateHashiPointMock(0, 1).Object]);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _3ConnectionsRule2_WhenIslandHasLessThanTwoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        // valid neighbor
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);

        var testIsland = SetupTestIsland(3, leftIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _3ConnectionsRule2_WhenNoNeighbors_ShouldNotTriggerRule()
    {
        // arrange
        var testIsland = SetupTestIsland(3);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void _3ConnectionsRule2_WhenAllNeighborsHaveMaxConnections_ShouldNotTriggerRule()
    {
        // arrange
        var leftIsland = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        leftIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var rightIsland = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        rightIsland.Setup(mock => mock.MaxConnectionsReached).Returns(true);

        var testIsland = SetupTestIsland(3, leftIsland, rightIsland);

        // act
        Session.Insert(testIsland.Object);
        Session.Fire();

        // assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}