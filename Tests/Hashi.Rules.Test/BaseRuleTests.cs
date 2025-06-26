using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hashi.Rules.Test;

/// <summary>
/// Unit tests for BaseRule class.
/// </summary>
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
        targetMock.Setup(x => x.AllConnections).Returns(new ObservableCollection<IHashiPoint> { sourceCoords, sourceCoords });

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

    #endregion

    #region Helper Methods

    private Mock<IIslandViewModel> CreateIslandMock(int x, int y, int maxConnections, bool maxConnectionsReached)
    {
        var islandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var coordinatesMock = CreateHashiPointMock(x, y);
        
        islandMock.Setup(m => m.Coordinates).Returns(coordinatesMock.Object);
        islandMock.Setup(m => m.MaxConnections).Returns(maxConnections);
        islandMock.Setup(m => m.MaxConnectionsReached).Returns(maxConnectionsReached);
        islandMock.Setup(m => m.AllConnections).Returns(new ObservableCollection<IHashiPoint>());
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
    public new bool DoCoordinatesMatch(IHashiPoint source, IHashiPoint target) => base.DoCoordinatesMatch(source, target);
}