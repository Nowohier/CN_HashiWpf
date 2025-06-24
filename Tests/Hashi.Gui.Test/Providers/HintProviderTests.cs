using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Providers;
using Hashi.Rules;
using Hashi.Rules.Extensions;
using Moq;
using NRules;
using NRules.RuleModel;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class HintProviderTests
{
    private Mock<IIslandProvider> islandProviderMock;
    private Mock<IDialogWrapper> dialogWrapperMock;
    private Mock<IRuleRepository> ruleRepositoryMock;
    private Mock<IRuleInfoProvider> ruleInfoProviderMock;
    private HintProvider sut;

    [SetUp]
    public void SetUp()
    {
        islandProviderMock = new Mock<IIslandProvider>(MockBehavior.Strict);
        dialogWrapperMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        ruleRepositoryMock = new Mock<IRuleRepository>(MockBehavior.Strict);
        ruleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);

        islandProviderMock.Setup(x => x.IslandsFlat).Returns(new List<IIslandViewModel>());

        // Setup common method calls 
        ruleInfoProviderMock.SetupProperty(x => x.AreRulesBeingApplied, false);
        // Note: CompileOne is an extension method and cannot be mocked directly
        ruleRepositoryMock.Setup(x => x.Compile()).Returns(Mock.Of<ISessionFactory>());
        dialogWrapperMock.Setup(x => x.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogButton>(), It.IsAny<DialogImage>())).Verifiable();

        sut = new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);
    }

    [Test]
    public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new HintProvider(
            islandProviderMock.Object,
            dialogWrapperMock.Object,
            ruleRepositoryMock.Object,
            ruleInfoProviderMock.Object);

        // Assert
        result.RuleInfoProvider.Should().Be(ruleInfoProviderMock.Object);
        result.Rules.Should().NotBeNull().And.NotBeEmpty();
    }

    [Test]
    public void Rules_WhenAccessed_ShouldContainRuleTypes()
    {
        // Arrange & Act
        var rules = sut.Rules;

        // Assert
        rules.Should().NotBeNull();
        rules.Should().NotBeEmpty();
        rules.Should().AllSatisfy(rule => rule.Name.Should().StartWith("_"));
        rules.Should().Contain(type => type == typeof(_1ConnectionRule1));
    }

    [Test]
    public void RuleInfoProvider_WhenAccessed_ShouldReturnInjectedProvider()
    {
        // Arrange & Act
        var ruleInfoProvider = sut.RuleInfoProvider;

        // Assert
        ruleInfoProvider.Should().Be(ruleInfoProviderMock.Object);
    }

    [Test]
    public void ResetSession_WhenSessionIsNull_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.ResetSession()).Should().NotThrow();
    }

    [Test]
    public void ResetSession_WhenSessionExists_ShouldResetRuleInfoProvider()
    {
        // Arrange
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(true);

        // Act
        sut.ResetSession();

        // Assert
        ruleInfoProviderMock.VerifySet(x => x.AreRulesBeingApplied = false, Times.Once);
    }

    [Test]
    public void GenerateHint_WhenCalledWithNullRule_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.GenerateHint(null!))
           .Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void GenerateHint_WhenRulesAreBeingApplied_ShouldReturnEarly()
    {
        // Arrange
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(true);
        var selectedRule = typeof(_1ConnectionRule1);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        ruleRepositoryMock.Verify(x => x.Compile(), Times.Never);
    }

    // Note: Tests for specific rule compilation (CompileOne) are not possible 
    // because CompileOne is an extension method that cannot be mocked with Moq.
    // These tests have been removed as they would require actual NRules infrastructure.

    [Test]
    public void GenerateHint_WhenCalledWithAllRulesType_ShouldCompileAllRules()
    {
        // Arrange
        ruleInfoProviderMock.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_0AllRules);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        ruleRepositoryMock.Setup(x => x.Compile())
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(1);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        ruleRepositoryMock.Verify(x => x.Compile(), Times.Once);
    }

    // Additional tests that rely on CompileOne extension method have been removed
    // as they cannot be properly mocked and tested in isolation.
}