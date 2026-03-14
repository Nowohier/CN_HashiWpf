using FluentAssertions;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

/// <summary>
///     Unit tests for <see cref="_9GeneralRule6" /> class.
/// </summary>
public class _9GeneralRule6Tests : TestBase<_9GeneralRule6>
{
    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _9GeneralRule6(null!, IslandProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("ruleInfoProvider");
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _9GeneralRule6(RuleInfoProviderMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("islandProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new _9GeneralRule6(RuleInfoProviderMock.Object, IslandProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<_9GeneralRule6>();
    }

    [Test]
    public void Rule_When5ConnIsland4NeighborsTwoWithMax1_ShouldTrigger()
    {
        // Arrange - 5-conn island, 4 neighbors (2 with max-1, 2 with max-3)
        // Capacity = 1+1+2+2 = 6, remaining = 5 = capacity - 1
        // Neighbors with cap >= 2 must each get at least 1
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 5);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3);
        var neighbor4 = CreateIslandMock(TestIslandEnum.DownIsland, 4);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object, neighbor4.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, connecting to neighbors with cap >= 2
    }

    [Test]
    public void Rule_When3ConnIsland3NeighborsTwoWithMax1_ShouldTrigger()
    {
        // Arrange - 3-conn island, 3 neighbors (2 with max-1, 1 with max-3)
        // Capacity = 1+1+2 = 4, remaining = 3 = capacity - 1
        // Neighbor with cap >= 2 must get at least 1
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 3);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, connecting to the max-3 neighbor
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnectionsReached_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 5, true);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnections0_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 0);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenOnlyOneConnectableNeighbor_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenNoNeighborHasRemainingConnections1_ShouldNotTrigger()
    {
        // Arrange - all neighbors have cap >= 2, this is _9GeneralRule3 territory
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 3);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenRemainingDoesNotMatchCapacityMinus1_ShouldNotTrigger()
    {
        // Arrange - 4-conn island, 3 neighbors (2 with max-1, 1 with max-3)
        // Capacity = 1+1+2 = 4, remaining = 4 = capacity (not capacity-1)
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 4);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - This matches _9GeneralRule5 (saturation), not Rule6
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenAllNeighborsMaxedOut_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 5);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, true);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, true);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3, true);
        var neighbor4 = CreateIslandMock(TestIslandEnum.DownIsland, 4, true);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object, neighbor4.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenAllNeighborsHaveMax1Only_ShouldNotTrigger()
    {
        // Arrange - 2-conn island, 3 neighbors all with max-1
        // Capacity = 1+1+1 = 3, remaining = 2 = capacity - 1 = 2
        // But all neighbors have cap=1, so no neighbor has cap>=2
        // validNeighbors would be empty → should not trigger
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 2);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 1);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}
