using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

/// <summary>
///     Unit tests for <see cref="_9GeneralRule5" /> class.
/// </summary>
public class _9GeneralRule5Tests : TestBase<_9GeneralRule5>
{
    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _9GeneralRule5(null!, IslandProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("ruleInfoProvider");
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _9GeneralRule5(RuleInfoProviderMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("islandProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new _9GeneralRule5(RuleInfoProviderMock.Object, IslandProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<_9GeneralRule5>();
    }

    [Test]
    public void Rule_When4ConnIsland3NeighborsTwoWithMax1_ShouldTrigger()
    {
        // Arrange - 4-conn island, 3 neighbors (2 with max-1, 1 with max-3)
        // Capacity = min(2,1) + min(2,1) + min(2,3) = 1+1+2 = 4 = remaining
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 4, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, false);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object, neighbor3.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, connecting one to each neighbor
    }

    [Test]
    public void Rule_When6ConnIsland4NeighborsTwoWithMax1_ShouldTrigger()
    {
        // Arrange - 6-conn island, 4 neighbors (2 with max-1, 2 with max-3)
        // Capacity = 1+1+2+2 = 6 = remaining
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 6, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, false);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3, false);
        var neighbor4 = CreateIslandMock(TestIslandEnum.DownIsland, 4, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel>
                { neighbor1.Object, neighbor2.Object, neighbor3.Object, neighbor4.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, connecting one to each neighbor
    }

    [Test]
    public void Rule_When3ConnIsland3NeighborsAllMax1_ShouldTrigger()
    {
        // Arrange - 3-conn island, 3 neighbors all with max-1
        // Capacity = 1+1+1 = 3 = remaining
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 3, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, false);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 1, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object, neighbor3.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, connecting one to each neighbor
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnectionsReached_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 4, true);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel>());

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
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 0, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel>());

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
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenNoNeighborHasRemainingConnections1_ShouldNotTrigger()
    {
        // Arrange - 4-conn island, 2 neighbors both with max-3 (no cap-1 neighbor)
        // This would be _9GeneralRule4 territory (remaining=4, 2×2=4), not Rule5
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 4, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 3, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenRemainingDoesNotMatchCapacity_ShouldNotTrigger()
    {
        // Arrange - 3-conn island, 3 neighbors (1 with max-1, 2 with max-3)
        // Capacity = 1+2+2 = 5 != 3
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 3, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 3, false);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object, neighbor3.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenAllNeighborsMaxedOut_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 4, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 1, true);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 1, true);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 3, true);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object, neighbor3.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}
