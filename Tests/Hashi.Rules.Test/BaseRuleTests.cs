using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for BaseRule class.
/// </summary>
[TestFixture]
public class BaseRuleTests
{
    private Mock<IIslandProvider> islandProviderMock;
    private Mock<IRuleInfoProvider> ruleInfoProviderMock;
    private TestableBaseRule baseRule;

    [SetUp]
    public void SetUp()
    {
        islandProviderMock = new Mock<IIslandProvider>(MockBehavior.Strict);
        ruleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);
        baseRule = new TestableBaseRule(ruleInfoProviderMock.Object, islandProviderMock.Object);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestableBaseRule(null!, islandProviderMock.Object);
        action.Should().Throw<ArgumentNullException>().WithParameterName("ruleInfoProvider");
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new TestableBaseRule(ruleInfoProviderMock.Object, null!);
        action.Should().Throw<ArgumentNullException>().WithParameterName("islandProvider");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var result = new TestableBaseRule(ruleInfoProviderMock.Object, islandProviderMock.Object);

        // Assert
        result.Should().NotBeNull();
    }

    #endregion

    #region EnsureRulesAreBeingApplied Tests

    [Test]
    public void EnsureRulesAreBeingApplied_WhenRulesNotBeingApplied_ShouldReturnFalse()
    {
        // Arrange
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(false);

        // Act
        var result = baseRule.EnsureRulesAreBeingApplied();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void EnsureRulesAreBeingApplied_WhenRulesBeingApplied_ShouldSetRuleMessageAndReturnTrue()
    {
        // Arrange
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(true);
        ruleInfoProviderMock.SetupSet(x => x.RuleMessage = It.IsAny<string>());

        // Act
        var result = baseRule.EnsureRulesAreBeingApplied();

        // Assert
        result.Should().BeTrue();
        ruleInfoProviderMock.VerifySet(x => x.RuleMessage = "Test Rule Message", Times.Once);
    }

    #endregion

    #region AddConnection Tests

    [Test]
    public void AddConnection_WhenRulesNotBeingApplied_ShouldNotExecuteConnection()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);
        var targetMock = CreateIslandMock(2, 1, 2, false);
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(false);

        // Act
        baseRule.AddConnection(sourceMock.Object, targetMock.Object);

        // Assert
        islandProviderMock.Verify(x => x.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), It.IsAny<HashiPointTypeEnum>()), Times.Never);
    }

    [Test]
    public void AddConnection_WhenValidConnection_ShouldExecuteConnectionAndFinalize()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);
        var targetMock = CreateIslandMock(2, 1, 2, false);
        
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(true);
        ruleInfoProviderMock.SetupSet(x => x.RuleMessage = It.IsAny<string>());
        islandProviderMock.Setup(x => x.AddConnection(sourceMock.Object, targetMock.Object, HashiPointTypeEnum.Hint));
        sourceMock.Setup(x => x.RefreshIslandColor());
        targetMock.Setup(x => x.RefreshIslandColor());

        // Act
        baseRule.AddConnection(sourceMock.Object, targetMock.Object);

        // Assert
        islandProviderMock.Verify(x => x.AddConnection(sourceMock.Object, targetMock.Object, HashiPointTypeEnum.Hint), Times.Once);
        sourceMock.Verify(x => x.RefreshIslandColor(), Times.Once);
        targetMock.Verify(x => x.RefreshIslandColor(), Times.Once);
    }

    #endregion

    #region ExecuteAddConnection Tests

    [Test]
    public void ExecuteAddConnection_WhenSourceIsNull_ShouldReturnFalse()
    {
        // Arrange
        var targetMock = CreateIslandMock(2, 1, 2, false);

        // Act
        var result = baseRule.ExecuteAddConnection(null, targetMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_WhenTargetIsNull_ShouldReturnFalse()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);

        // Act
        var result = baseRule.ExecuteAddConnection(sourceMock.Object, null);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_WhenSourceAndTargetAreSame_ShouldReturnFalse()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);

        // Act
        var result = baseRule.ExecuteAddConnection(sourceMock.Object, sourceMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_WhenSourceMaxConnectionsReached_ShouldReturnFalse()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, true);
        var targetMock = CreateIslandMock(2, 1, 2, false);

        // Act
        var result = baseRule.ExecuteAddConnection(sourceMock.Object, targetMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_WhenTargetMaxConnectionsReached_ShouldReturnFalse()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);
        var targetMock = CreateIslandMock(2, 1, 2, true);

        // Act
        var result = baseRule.ExecuteAddConnection(sourceMock.Object, targetMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_WhenMaxConnectionsBetweenSourceAndTargetReached_ShouldReturnFalse()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 4, false);
        var targetMock = CreateIslandMock(2, 1, 4, false);
        
        // Setup two existing connections from target to source
        var sourceCoords = sourceMock.Object.Coordinates;
        targetMock.Setup(x => x.AllConnections).Returns([sourceCoords, sourceCoords]);

        // Act
        var result = baseRule.ExecuteAddConnection(sourceMock.Object, targetMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ExecuteAddConnection_WhenValidConnection_ShouldReturnTrueAndAddConnection()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);
        var targetMock = CreateIslandMock(2, 1, 2, false);
        islandProviderMock.Setup(x => x.AddConnection(sourceMock.Object, targetMock.Object, HashiPointTypeEnum.Hint));

        // Act
        var result = baseRule.ExecuteAddConnection(sourceMock.Object, targetMock.Object);

        // Assert
        result.Should().BeTrue();
        islandProviderMock.Verify(x => x.AddConnection(sourceMock.Object, targetMock.Object, HashiPointTypeEnum.Hint), Times.Once);
    }

    #endregion

    #region FinalizeConnection Tests

    [Test]
    public void FinalizeConnection_WhenSourceIsNull_ShouldNotThrow()
    {
        // Arrange
        var targetMock = CreateIslandMock(2, 1, 2, false);
        targetMock.Setup(x => x.RefreshIslandColor());

        // Act & Assert
        var action = () => baseRule.FinalizeConnection(null, targetMock.Object);
        action.Should().NotThrow();
        
        targetMock.Verify(x => x.RefreshIslandColor(), Times.Once);
    }

    [Test]
    public void FinalizeConnection_WhenTargetIsNull_ShouldNotThrow()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);
        sourceMock.Setup(x => x.RefreshIslandColor());

        // Act & Assert
        var action = () => baseRule.FinalizeConnection(sourceMock.Object, null);
        action.Should().NotThrow();
        
        sourceMock.Verify(x => x.RefreshIslandColor(), Times.Once);
    }

    [Test]
    public void FinalizeConnection_WhenValidSourceAndTarget_ShouldRefreshBothColors()
    {
        // Arrange
        var sourceMock = CreateIslandMock(1, 1, 2, false);
        var targetMock = CreateIslandMock(2, 1, 2, false);
        sourceMock.Setup(x => x.RefreshIslandColor());
        targetMock.Setup(x => x.RefreshIslandColor());

        // Act
        baseRule.FinalizeConnection(sourceMock.Object, targetMock.Object);

        // Assert
        sourceMock.Verify(x => x.RefreshIslandColor(), Times.Once);
        targetMock.Verify(x => x.RefreshIslandColor(), Times.Once);
    }

    #endregion

    #region DoCoordinatesMatch Tests

    [Test]
    public void DoCoordinatesMatch_WhenCoordinatesMatch_ShouldReturnTrue()
    {
        // Arrange
        var coord1Mock = CreateHashiPointMock(1, 2);
        var coord2Mock = CreateHashiPointMock(1, 2);

        // Act
        var result = baseRule.DoCoordinatesMatch(coord1Mock.Object, coord2Mock.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void DoCoordinatesMatch_WhenCoordinatesDontMatch_ShouldReturnFalse()
    {
        // Arrange
        var coord1Mock = CreateHashiPointMock(1, 2);
        var coord2Mock = CreateHashiPointMock(3, 4);

        // Act
        var result = baseRule.DoCoordinatesMatch(coord1Mock.Object, coord2Mock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void AreAllNeighborsConnected_WhenAllNeighborsConnected_ShouldReturnTrue()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var neighbor2 = CreateIslandMock(1, 2, 2, false);
        
        // The neighbors should contain the source's coordinates in their connections
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.AreAllNeighborsConnected(source.Object, neighbors);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void AreAllNeighborsConnected_WhenNotAllNeighborsConnected_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var neighbor2 = CreateIslandMock(1, 2, 2, false);
        
        // Only neighbor1 contains the source's coordinates
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.AreAllNeighborsConnected(source.Object, neighbors);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CountConnectionsToNeighbors_WhenNeighborsHaveConnections_ShouldReturnCorrectCount()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var neighbor2 = CreateIslandMock(1, 2, 2, false);
        
        // neighbor1 has 2 connections to source, neighbor2 has 1 connection to source
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates, source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.CountConnectionsToNeighbors(source.Object, neighbors);

        // Assert
        result.Should().Be(3);
    }

    [Test]
    public void CountConnectionsToNeighbors_WhenNoConnections_ShouldReturnZero()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var neighbor2 = CreateIslandMock(1, 2, 2, false);
        
        neighbor1.Setup(x => x.AllConnections).Returns([]);
        neighbor2.Setup(x => x.AllConnections).Returns([]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.CountConnectionsToNeighbors(source.Object, neighbors);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void GetConnectedNeighbors_WhenAmountConnectionsIsNull_ShouldReturnAllConnectedNeighbors()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var neighbor2 = CreateIslandMock(1, 2, 2, false);
        var neighbor3 = CreateIslandMock(3, 1, 2, false);
        
        // neighbor1 and neighbor2 have connections to source, neighbor3 doesn't
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        neighbor3.Setup(x => x.AllConnections).Returns([]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object, neighbor3.Object };

        // Act
        var result = baseRule.GetConnectedNeighbors(source.Object, neighbors, null);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(neighbor1.Object);
        result.Should().Contain(neighbor2.Object);
    }

    [Test]
    public void GetConnectedNeighbors_WhenAmountConnectionsSpecified_ShouldReturnMatchingNeighbors()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var neighbor2 = CreateIslandMock(1, 2, 2, false);
        
        // neighbor1 has 2 connections to source, neighbor2 has 1 connection to source
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates, source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.GetConnectedNeighbors(source.Object, neighbors, 2);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighbor1.Object);
    }

    [Test]
    public void GetMaxedOutConnectedNeighbors_WhenNeighborsMaxedOut_ShouldReturnMaxedOutNeighbors()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, true); // Maxed out
        var neighbor2 = CreateIslandMock(1, 2, 2, false); // Not maxed out
        
        // Both neighbors have connections to source
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.GetMaxedOutConnectedNeighbors(source.Object, neighbors, null);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighbor1.Object);
    }

    [Test]
    public void SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor_WhenNoIsolatedGroups_ShouldReturnNull()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false);
        var connectableNeighbors = new List<IIslandViewModel> { neighbor1.Object };
        var allNeighbors = new List<IIslandViewModel> { neighbor1.Object };
        
        islandProviderMock.Setup(x => x.CountIsolatedIslandGroups()).Returns(0);
        islandProviderMock.Setup(x => x.AddConnection(It.IsAny<IIslandViewModel>(), It.IsAny<IIslandViewModel>(), It.IsAny<HashiPointTypeEnum>()));
        islandProviderMock.Setup(x => x.RemoveAllBridges(It.IsAny<HashiPointTypeEnum>()));

        // Act
        var result = baseRule.SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(source.Object, connectableNeighbors, allNeighbors);

        // Assert
        result.Should().BeNull();
        islandProviderMock.Verify(x => x.RemoveAllBridges(HashiPointTypeEnum.Test), Times.Once);
    }

    [Test]
    public void ExecuteAddConnection_WhenSourceAndTargetAreAlreadyConnected_ShouldReturnFalse()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var target = CreateIslandMock(2, 1, 2, false);
        
        // Setup that target already has 2 connections to source (max connections for a bridge pair)
        target.Setup(x => x.AllConnections).Returns([source.Object.Coordinates, source.Object.Coordinates]);
        source.Setup(x => x.AllConnections).Returns([]);

        // Act
        var result = baseRule.ExecuteAddConnection(source.Object, target.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetConnectableNeighborsWithoutConnection_WhenNeighborHasConnectionToSource_ShouldExcludeNeighbor()
    {
        // Arrange
        var source = CreateIslandMock(1, 1, 2, false);
        var neighbor1 = CreateIslandMock(2, 1, 2, false); // Has connection to source
        var neighbor2 = CreateIslandMock(1, 2, 2, false); // No connection to source
        
        // neighbor1 has a connection to source coordinates
        neighbor1.Setup(x => x.AllConnections).Returns([source.Object.Coordinates]);
        neighbor2.Setup(x => x.AllConnections).Returns([]);
        
        var neighbors = new List<IIslandViewModel> { neighbor1.Object, neighbor2.Object };

        // Act
        var result = baseRule.GetConnectableNeighborsWithoutConnection(source.Object, neighbors);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(neighbor2.Object);
        result.Should().NotContain(neighbor1.Object);
    }

    #endregion

    #region Helper Methods

    private Mock<IIslandViewModel> CreateIslandMock(int x, int y, int maxConnections, bool maxConnectionsReached)
    {
        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var coordinatesMock = CreateHashiPointMock(x, y);
        
        islandMock.Setup(m => m.Coordinates).Returns(coordinatesMock.Object);
        islandMock.Setup(m => m.MaxConnections).Returns(maxConnections);
        islandMock.Setup(m => m.MaxConnectionsReached).Returns(maxConnectionsReached);
        islandMock.Setup(m => m.AllConnections).Returns([]);
        islandMock.Setup(m => m.RemainingConnections).Returns(maxConnectionsReached ? 0 : maxConnections);
        
        return islandMock;
    }

    private Mock<IHashiPoint> CreateHashiPointMock(int x, int y)
    {
        var hashiPointMock = new Mock<IHashiPoint>(MockBehavior.Strict);
        hashiPointMock.Setup(m => m.X).Returns(x);
        hashiPointMock.Setup(m => m.Y).Returns(y);
        return hashiPointMock;
    }

    #endregion
}

/// <summary>
/// Concrete implementation of BaseRule for testing purposes.
/// </summary>
[ExcludeFromCodeCoverage]
public class TestableBaseRule : BaseRule
{
    public TestableBaseRule(IRuleInfoProvider ruleInfoProvider, IIslandProvider islandProvider) 
        : base(ruleInfoProvider, islandProvider)
    {
    }

    protected override string RuleMessage => "Test Rule Message";

    public override void Define()
    {
        // No implementation needed for testing
    }

    // Expose internal methods for testing
    public new bool EnsureRulesAreBeingApplied() => base.EnsureRulesAreBeingApplied();
    public new void AddConnection(IIslandViewModel source, IIslandViewModel target) => base.AddConnection(source, target);
    public new bool ExecuteAddConnection(IIslandViewModel? source, IIslandViewModel? target) => base.ExecuteAddConnection(source, target);
    public new void FinalizeConnection(IIslandViewModel? source, IIslandViewModel? target) => base.FinalizeConnection(source, target);
    public bool DoCoordinatesMatch(IHashiPoint source, IHashiPoint target) => Analyzer.DoCoordinatesMatch(source, target);
    public bool AreAllNeighborsConnected(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors) => Analyzer.AreAllNeighborsConnected(source, allNeighbors);
    public int CountConnectionsToNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> neighbors) => Analyzer.CountConnectionsToNeighbors(source, neighbors);
    public List<IIslandViewModel> GetConnectedNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections) => Analyzer.GetConnectedNeighbors(source, allNeighbors, amountConnections);
    public List<IIslandViewModel> GetMaxedOutConnectedNeighbors(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors, int? amountConnections) => Analyzer.GetMaxedOutConnectedNeighbors(source, allNeighbors, amountConnections);
    public new IIslandViewModel? SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(IIslandViewModel source, List<IIslandViewModel> connectableNeighbors, List<IIslandViewModel> allNeighbors) => base.SetTestConnectionAndIfGroupIsIsolatedReturnValidNeighbor(source, connectableNeighbors, allNeighbors);
    public List<IIslandViewModel> GetConnectableNeighborsWithoutConnection(IIslandViewModel source, IEnumerable<IIslandViewModel> allNeighbors) => Analyzer.GetConnectableNeighborsWithoutConnection(source, allNeighbors);
}