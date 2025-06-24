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
    private Mock<IIslandProvider> mockIslandProvider;
    private Mock<IDialogWrapper> mockDialogWrapper;
    private Mock<IRuleRepository> mockRuleRepository;
    private Mock<IRuleInfoProvider> mockRuleInfoProvider;
    private HintProvider sut;

    [SetUp]
    public void SetUp()
    {
        mockIslandProvider = new Mock<IIslandProvider>();
        mockDialogWrapper = new Mock<IDialogWrapper>();
        mockRuleRepository = new Mock<IRuleRepository>();
        mockRuleInfoProvider = new Mock<IRuleInfoProvider>();

        mockIslandProvider.Setup(x => x.IslandsFlat).Returns(new List<IIslandViewModel>());

        sut = new HintProvider(
            mockIslandProvider.Object,
            mockDialogWrapper.Object,
            mockRuleRepository.Object,
            mockRuleInfoProvider.Object);
    }

    [Test]
    public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new HintProvider(
            mockIslandProvider.Object,
            mockDialogWrapper.Object,
            mockRuleRepository.Object,
            mockRuleInfoProvider.Object);

        // Assert
        result.RuleInfoProvider.Should().Be(mockRuleInfoProvider.Object);
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
        ruleInfoProvider.Should().Be(mockRuleInfoProvider.Object);
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
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(true);

        // Act
        sut.ResetSession();

        // Assert
        mockRuleInfoProvider.VerifySet(x => x.AreRulesBeingApplied = false, Times.Once);
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
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(true);
        var selectedRule = typeof(_1ConnectionRule1);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockRuleRepository.Verify(x => x.CompileOne(It.IsAny<string>()), Times.Never);
        mockRuleRepository.Verify(x => x.Compile(), Times.Never);
    }

    [Test]
    public void GenerateHint_WhenCalledWithValidRule_ShouldSetRulesBeingApplied()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_1ConnectionRule1);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        mockRuleRepository.Setup(x => x.CompileOne(selectedRule.FullName!))
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(0);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockRuleInfoProvider.VerifySet(x => x.AreRulesBeingApplied = true, Times.Once);
        mockRuleInfoProvider.VerifySet(x => x.AreRulesBeingApplied = false, Times.Once);
    }

    [Test]
    public void GenerateHint_WhenCalledWithSpecificRule_ShouldCompileOneRule()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_1ConnectionRule1);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        mockRuleRepository.Setup(x => x.CompileOne(selectedRule.FullName!))
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(1);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockRuleRepository.Verify(x => x.CompileOne(selectedRule.FullName!), Times.Once);
        mockRuleRepository.Verify(x => x.Compile(), Times.Never);
    }

    [Test]
    public void GenerateHint_WhenCalledWithAllRulesType_ShouldCompileAllRules()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_0AllRules);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        mockRuleRepository.Setup(x => x.Compile())
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(1);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockRuleRepository.Verify(x => x.Compile(), Times.Once);
        mockRuleRepository.Verify(x => x.CompileOne(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GenerateHint_WhenNoRulesFired_ShouldShowNoHintsDialog()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_1ConnectionRule1);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        mockRuleRepository.Setup(x => x.CompileOne(selectedRule.FullName!))
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(0); // No rules fired

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockDialogWrapper.Verify(x => x.Show(
            It.IsAny<string>(),
            It.IsAny<string>(),
            DialogButton.Ok,
            DialogImage.Information), Times.Once);
    }

    [Test]
    public void GenerateHint_WhenRulesFired_ShouldNotShowNoHintsDialog()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_1ConnectionRule1);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        mockRuleRepository.Setup(x => x.CompileOne(selectedRule.FullName!))
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(1); // Rules fired

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockDialogWrapper.Verify(x => x.Show(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DialogButton>(),
            It.IsAny<DialogImage>()), Times.Never);
    }

    [Test]
    public void GenerateHint_WhenSessionExists_ShouldUpdateAllIslands()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_1ConnectionRule1);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        // First call to create session
        mockRuleRepository.Setup(x => x.CompileOne(selectedRule.FullName!))
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(1);

        sut.GenerateHint(selectedRule); // Create session

        // Second call should update
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockSession.Verify(x => x.UpdateAll(mockIslandProvider.Object.IslandsFlat), Times.Once);
        mockSession.Verify(x => x.InsertAll(mockIslandProvider.Object.IslandsFlat), Times.Once);
    }

    [Test]
    public void GenerateHint_WhenCalled_ShouldInsertAllIslands()
    {
        // Arrange
        mockRuleInfoProvider.Setup(x => x.AreRulesBeingApplied).Returns(false);
        var selectedRule = typeof(_1ConnectionRule1);
        var mockSessionFactory = new Mock<ISessionFactory>();
        var mockSession = new Mock<ISession>();

        mockRuleRepository.Setup(x => x.CompileOne(selectedRule.FullName!))
                         .Returns(mockSessionFactory.Object);
        mockSessionFactory.Setup(x => x.CreateSession()).Returns(mockSession.Object);
        mockSession.Setup(x => x.Fire()).Returns(1);

        // Act
        sut.GenerateHint(selectedRule);

        // Assert
        mockSession.Verify(x => x.InsertAll(mockIslandProvider.Object.IslandsFlat), Times.Once);
    }
}