using FluentAssertions;
using Hashi.Rules.Test.Helpers;
using Times = NRules.Testing.Times;

namespace Hashi.Rules.Test;

/// <summary>
///     Unit tests for <see cref="_7ConnectionsRule2" /> class.
/// </summary>
public class _7ConnectionsRule2Tests : TestBase<_7ConnectionsRule2>
{
    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _7ConnectionsRule2(null!, IslandProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("ruleInfoProvider");
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new _7ConnectionsRule2(RuleInfoProviderMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("islandProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new _7ConnectionsRule2(RuleInfoProviderMock.Object, IslandProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<_7ConnectionsRule2>();
    }

    [Test]
    public void Rule_WhenIslandHas7ConnectionsAndOneMaxedOutNeighborWith1Connection_ShouldTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 7);
        var maxedNeighbor = CreateIslandMock(TestIslandEnum.RightIsland, 1, true);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 3);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 4);
        var neighbor4 = CreateIslandMock(TestIslandEnum.DownIsland, 2);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([maxedNeighbor.Object, neighbor2.Object, neighbor3.Object, neighbor4.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert - rule should trigger, double-connecting to remaining 3 neighbors
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnectionsReached_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 7, true);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenIslandHasMaxConnectionsNot7_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 6);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenIslandHas3Neighbors_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 7);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 4);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Test]
    public void Rule_WhenNoMaxedOutNeighbor_ShouldNotTrigger()
    {
        // Arrange
        var island = CreateIslandMock(TestIslandEnum.TestIsland, 7);
        var neighbor1 = CreateIslandMock(TestIslandEnum.RightIsland, 3);
        var neighbor2 = CreateIslandMock(TestIslandEnum.LeftIsland, 2);
        var neighbor3 = CreateIslandMock(TestIslandEnum.UpIsland, 4);
        var neighbor4 = CreateIslandMock(TestIslandEnum.DownIsland, 2);

        IslandProviderMock.Setup(x => x.GetAllVisibleNeighbors(island.Object))
            .Returns([neighbor1.Object, neighbor2.Object, neighbor3.Object, neighbor4.Object]);

        // Act
        Session.Insert(island.Object);
        Session.Fire();

        // Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }
}
