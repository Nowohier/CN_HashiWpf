using FluentAssertions;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Providers;
using Hashi.Rules;
using Moq;
using NRules;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class HintProviderTests
{
    private HintProvider hintProvider;
    private Mock<IIslandProvider> islandProviderMock;
    private Mock<IDialogWrapper> dialogWrapperMock;
    private Mock<IRuleRepository> ruleRepositoryMock;
    private Mock<IRuleInfoProvider> ruleInfoProviderMock;

    [SetUp]
    public void SetUp()
    {
        islandProviderMock = new Mock<IIslandProvider>(MockBehavior.Strict);
        dialogWrapperMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        ruleRepositoryMock = new Mock<IRuleRepository>(MockBehavior.Strict);
        ruleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);

        hintProvider = new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Act
        var result = new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);

        // Assert
        result.RuleInfoProvider.Should().Be(ruleInfoProviderMock.Object);
        result.Rules.Should().NotBeNull();
        result.Rules.Should().NotBeEmpty();
    }

    [Test]
    public void Constructor_WhenIslandProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HintProvider(
            null!,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenDialogWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HintProvider(
            islandProviderMock.Object,
            null!,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenRuleRepositoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            null!,
            ruleInfoProviderMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenRuleInfoProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            null!);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void RuleInfoProvider_WhenAccessed_ShouldReturnConstructorParameter()
    {
        // Act
        var result = hintProvider.RuleInfoProvider;

        // Assert
        result.Should().Be(ruleInfoProviderMock.Object);
    }

    [Test]
    public void Rules_WhenAccessed_ShouldReturnListOfRuleTypes()
    {
        // Act
        var result = hintProvider.Rules;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IList<Type>>();
        result.Should().NotBeEmpty();
    }

    [Test]
    public void Rules_WhenAccessed_ShouldContainTypesStartingWithUnderscore()
    {
        // Act
        var result = hintProvider.Rules;

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(type => type.Name.StartsWith('_'));
    }

    [Test]
    public void ResetSession_WhenSessionIsNull_ShouldNotThrow()
    {
        // Act & Assert
        var act = () => hintProvider.ResetSession();
        act.Should().NotThrow();
    }

    [Test]
    public void ResetSession_WhenCalled_ShouldSetAreRulesBeingAppliedToFalse()
    {
        // Arrange
        ruleInfoProviderMock.SetupSet(x => x.AreRulesBeingApplied = false);

        // Act
        hintProvider.ResetSession();

        // Assert
        ruleInfoProviderMock.VerifySet(x => x.AreRulesBeingApplied = false, Times.Once);
    }

    [Test]
    public void HintProvider_ShouldImplementIHintProvider()
    {
        // Act & Assert
        hintProvider.Should().BeAssignableTo<IHintProvider>();
    }

    [Test]
    public void Rules_WhenAccessedMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var result1 = hintProvider.Rules;
        var result2 = hintProvider.Rules;

        // Assert
        result1.Should().BeSameAs(result2);
    }

    [Test]
    public void Rules_WhenAccessed_ShouldContainRuleTypesFromAssembly()
    {
        // Act
        var result = hintProvider.Rules;

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(type => type.Name.Contains("ConnectionRule"));
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Arrange
        var secondRuleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);

        // Act
        var provider1 = new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);

        var provider2 = new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            secondRuleInfoProviderMock.Object);

        // Assert
        provider1.Should().NotBeSameAs(provider2);
        provider1.RuleInfoProvider.Should().NotBeSameAs(provider2.RuleInfoProvider);
    }

    [Test]
    public void Rules_WhenAccessed_ShouldNotBeEmpty()
    {
        // Act
        var result = hintProvider.Rules;

        // Assert
        result.Should().NotBeEmpty();
        result.Count.Should().BeGreaterThan(0);
    }
}