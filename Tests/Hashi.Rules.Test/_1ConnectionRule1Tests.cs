using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Rules.Test.Helpers;
using Moq;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for _1ConnectionRule1 class.
/// </summary>
public class _1ConnectionRule1Tests : TestBase<_1ConnectionRule1>
{
    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _1ConnectionRule1(null!, IslandProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("ruleInfoProvider");
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _1ConnectionRule1(RuleInfoProviderMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("islandProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new _1ConnectionRule1(RuleInfoProviderMock.Object, IslandProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<_1ConnectionRule1>();
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnections1AndNoValidNeighbors_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1, true); // Max connections already reached
        
        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel>());

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnections1AndOneValidNeighbor_ShouldTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1, false);
        var neighbor = CreateIslandMock(TestIslandEnum.RightIsland, 2, false);
        
        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Once));
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnections1AndMultipleValidNeighbors_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1, false);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 2, false);
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
    public void Rule_WhenIslandHasMaxConnections1AndNeighborAlsoHasMaxConnections1_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1, false);
        var neighbor = CreateIslandMock(TestIslandEnum.RightIsland, 1, false); // Neighbor also has max 1
        
        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnections1AndNeighborHasMaxConnectionsReached_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 1, false);
        var neighbor = CreateIslandMock(TestIslandEnum.RightIsland, 2, true); // Neighbor has max connections reached
        
        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenIslandHasMoreThanOneConnection_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 2, false); // Max connections > 1
        var neighbor = CreateIslandMock(TestIslandEnum.RightIsland, 2, false);
        
        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns(new List<IIslandViewModel> { neighbor.Object });

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}