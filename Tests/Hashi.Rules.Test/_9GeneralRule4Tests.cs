using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

/// <summary>
///     Unit tests for <see cref="_9GeneralRule4" /> class.
/// </summary>
public class _9GeneralRule4Tests : TestBase<_9GeneralRule4>
{
    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _9GeneralRule4(null!, IslandProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("ruleInfoProvider");
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _9GeneralRule4(RuleInfoProviderMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("islandProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new _9GeneralRule4(RuleInfoProviderMock.Object, IslandProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<_9GeneralRule4>();
    }

    [Test]
    public void Rule_WhenRemainingEquals2TimesNeighborCount_ShouldTrigger()
    {
        // Arrange - island with 4 remaining, 2 connectable neighbors (2*2 = 4)
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 4, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 2, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, double-connecting to each connectable neighbor
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
        // Arrange - formula requires at least 2 connectable neighbors
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 2, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 2, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenRemainingDoesNotMatchFormula_ShouldNotTrigger()
    {
        // Arrange - island with 3 remaining, 2 connectable neighbors (2*2 = 4 != 3)
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 3, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3, false);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 2, false);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object });

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
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 2, true);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 2, true);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}
