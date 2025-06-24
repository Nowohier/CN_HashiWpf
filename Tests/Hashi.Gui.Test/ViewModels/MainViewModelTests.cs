using FluentAssertions;
using Hashi.Enums;
using Hashi.Generator.Interfaces;
using Hashi.Gui.Interfaces.Managers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.ViewModels;
using Hashi.Logging.Interfaces;
using Hashi.Rules;
using Moq;
using System.Windows.Media;

namespace Hashi.Gui.Test.ViewModels;

[TestFixture]
public class MainViewModelTests
{
    private Mock<Func<SolidColorBrush, IHashiBrush>> brushFactoryMock;
    private Mock<IDialogWrapper> dialogWrapperMock;
    private Mock<IHashiGenerator> hashiGeneratorMock;
    private Mock<ISettingsProvider> settingsProviderMock;
    private Mock<ITimerProvider> timerProviderMock;
    private Mock<IIslandProvider> islandProviderMock;
    private Mock<IHintProvider> hintProviderMock;
    private Mock<ITestSolutionProvider> testSolutionProviderMock;
    private Mock<IResourceManager> resourceManagerMock;
    private Mock<ILoggerFactory> loggerFactoryMock;
    private Mock<ILogger> loggerMock;
    private Mock<ISettingsViewModel> settingsMock;
    private Mock<IRuleInfoProvider> ruleInfoProviderMock;
    private MainViewModel sut;

    [SetUp]
    public void SetUp()
    {
        brushFactoryMock = new Mock<Func<SolidColorBrush, IHashiBrush>>(MockBehavior.Strict);
        dialogWrapperMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        hashiGeneratorMock = new Mock<IHashiGenerator>(MockBehavior.Strict);
        settingsProviderMock = new Mock<ISettingsProvider>(MockBehavior.Strict);
        timerProviderMock = new Mock<ITimerProvider>(MockBehavior.Strict);
        islandProviderMock = new Mock<IIslandProvider>(MockBehavior.Strict);
        hintProviderMock = new Mock<IHintProvider>(MockBehavior.Strict);
        testSolutionProviderMock = new Mock<ITestSolutionProvider>(MockBehavior.Strict);
        resourceManagerMock = new Mock<IResourceManager>(MockBehavior.Strict);
        loggerFactoryMock = new Mock<ILoggerFactory>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger>(MockBehavior.Strict);
        settingsMock = new Mock<ISettingsViewModel>(MockBehavior.Strict);
        ruleInfoProviderMock = new Mock<IRuleInfoProvider>(MockBehavior.Strict);

        loggerFactoryMock.Setup(x => x.CreateLogger<MainViewModel>()).Returns(loggerMock.Object);
        settingsProviderMock.Setup(x => x.Settings).Returns(settingsMock.Object);
        hintProviderMock.Setup(x => x.RuleInfoProvider).Returns(ruleInfoProviderMock.Object);
        hintProviderMock.Setup(x => x.Rules).Returns(new List<Type> { typeof(_1ConnectionRule1), typeof(_2ConnectionsRule1) });
        settingsMock.Setup(x => x.HighScores).Returns([]);

        var hashiBrushMock = new Mock<IHashiBrush>(MockBehavior.Strict);
        brushFactoryMock.Setup(x => x.Invoke(It.IsAny<SolidColorBrush>())).Returns(hashiBrushMock.Object);

        // Setup logger methods
        loggerMock.Setup(x => x.Info(It.IsAny<string>())).Verifiable();
        loggerMock.Setup(x => x.Debug(It.IsAny<string>())).Verifiable();
        loggerMock.Setup(x => x.Error(It.IsAny<string>())).Verifiable();
        loggerMock.Setup(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>())).Verifiable();

        // Setup ResourceManager methods
        resourceManagerMock.Setup(x => x.PrepareUi()).Verifiable();
        resourceManagerMock.Setup(x => x.ResetSettingsAndLoadFromDefault()).Verifiable();

        // Setup RuleInfoProvider properties
        ruleInfoProviderMock.SetupProperty(x => x.RuleMessage, string.Empty);
        ruleInfoProviderMock.SetupProperty(x => x.AreRulesBeingApplied, false);

        // Setup HintProvider methods
        hintProviderMock.Setup(x => x.ResetSession()).Verifiable();
        hintProviderMock.Setup(x => x.GetHintForSelectedRule()).Verifiable();
        hintProviderMock.Setup(x => x.GetHintForAllRules()).Verifiable();

        timerProviderMock.Setup(x => x.Timer).Returns(new System.Diagnostics.Stopwatch());
        timerProviderMock.Setup(x => x.IsTimerRunning).Returns(false);
        
        islandProviderMock.Setup(x => x.Islands).Returns([]);
        islandProviderMock.Setup(x => x.IslandsFlat).Returns([]);
        
        testSolutionProviderMock.Setup(x => x.HashiFieldReference).Returns([]);
        testSolutionProviderMock.Setup(x => x.SolutionProviders).Returns([]);
        testSolutionProviderMock.SetupProperty(x => x.SelectedSolutionProvider);
        
        // Setup provider method calls
        timerProviderMock.Setup(x => x.StartTimer()).Verifiable();
        timerProviderMock.Setup(x => x.StopTimer()).Verifiable();
        islandProviderMock.Setup(x => x.InitializeNewSolution(It.IsAny<ISolutionProvider>())).Verifiable();
        islandProviderMock.Setup(x => x.RemoveAllHighlights()).Verifiable();
        testSolutionProviderMock.Setup(x => x.SaveTestFields()).Verifiable();
        testSolutionProviderMock.Setup(x => x.ResetSettings()).Verifiable();
        testSolutionProviderMock.Setup(x => x.ConvertIslandsToSolutionProvider(It.IsAny<IEnumerable<IIslandViewModel>>())).Verifiable();
        hashiGeneratorMock.Setup(x => x.Generate(It.IsAny<DifficultyEnum>())).Returns(Mock.Of<ISolutionProvider>());

        // Setup DialogWrapper methods
        dialogWrapperMock.Setup(x => x.Show(It.IsAny<string>(), It.IsAny<string>())).Verifiable();

        // Setup Settings methods
        settingsProviderMock.Setup(x => x.SaveSettings()).Verifiable();

        sut = new MainViewModel(
            brushFactoryMock.Object,
            dialogWrapperMock.Object,
            hashiGeneratorMock.Object,
            settingsProviderMock.Object,
            timerProviderMock.Object,
            islandProviderMock.Object,
            hintProviderMock.Object,
            testSolutionProviderMock.Object,
            resourceManagerMock.Object,
            loggerFactoryMock.Object);
    }

    [Test]
    public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new MainViewModel(
            brushFactoryMock.Object,
            dialogWrapperMock.Object,
            hashiGeneratorMock.Object,
            settingsProviderMock.Object,
            timerProviderMock.Object,
            islandProviderMock.Object,
            hintProviderMock.Object,
            testSolutionProviderMock.Object,
            resourceManagerMock.Object,
            loggerFactoryMock.Object);

        // Assert
        result.SettingsProvider.Should().Be(settingsProviderMock.Object);
        result.TimerProvider.Should().Be(timerProviderMock.Object);
        result.IslandProvider.Should().Be(islandProviderMock.Object);
        result.HintProvider.Should().Be(hintProviderMock.Object);
        result.TestSolutionProvider.Should().Be(testSolutionProviderMock.Object);
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateCommands()
    {
        // Arrange & Act & Assert
        sut.CreateNewGameCommand.Should().NotBeNull();
        sut.RemoveAllBridgesCommand.Should().NotBeNull();
        sut.GenerateHintCommand.Should().NotBeNull();
        sut.UndoCommand.Should().NotBeNull();
        sut.RedoCommand.Should().NotBeNull();
        sut.WindowMouseClickedCommand.Should().NotBeNull();
        sut.ChangeLanguageCommand.Should().NotBeNull();
        sut.ToggleTestFieldCommand.Should().NotBeNull();
        sut.ResetTestFieldCommand.Should().NotBeNull();
        sut.SaveTestFieldCommand.Should().NotBeNull();
        sut.DeleteTestFieldCommand.Should().NotBeNull();
        sut.CreateTestFieldCommand.Should().NotBeNull();
        sut.ResetAllSettingsToDefaultCommand.Should().NotBeNull();
    }

    [Test]
    public void Initialize_WhenCalled_ShouldPrepareUi()
    {
        // Arrange & Act
        sut.Initialize();

        // Assert
        resourceManagerMock.Verify(x => x.PrepareUi(), Times.Once);
    }

    [Test]
    public void Initialize_WhenCalled_ShouldSetSelectedRule()
    {
        // Arrange & Act
        sut.Initialize();

        // Assert
        sut.SelectedRule.Should().Be(typeof(_1ConnectionRule1));
    }

    [Test]
    public void Initialize_WhenCalled_ShouldSetWindowColorBrush()
    {
        // Arrange & Act
        sut.Initialize();

        // Assert
        sut.WindowColorBrush.Should().NotBeNull();
        brushFactoryMock.Verify(x => x.Invoke(It.IsAny<SolidColorBrush>()), Times.Once);
    }

    [Test]
    public void Title_WhenIsTestFieldModeIsFalse_ShouldReturnBasicTitle()
    {
        // Arrange & Act
        var title = sut.Title;

        // Assert
        title.Should().Be("Hashiwokakero");
    }

    [Test]
    public void Title_WhenIsTestFieldModeIsTrue_ShouldReturnTestModeTitle()
    {
        // Arrange
        sut.IsTestFieldMode = true;

        // Act
        var title = sut.Title;

        // Assert
        title.Should().Be("Hashiwokakero - Testmode");
    }

    [Test]
    public void SelectedDifficulty_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newDifficulty = DifficultyEnum.Hard3;

        // Act
        sut.SelectedDifficulty = newDifficulty;

        // Assert
        sut.SelectedDifficulty.Should().Be(newDifficulty);
    }

    [Test]
    public void SelectedRule_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newRule = typeof(_2ConnectionsRule1);

        // Act
        sut.SelectedRule = newRule;

        // Assert
        sut.SelectedRule.Should().Be(newRule);
    }

    [Test]
    public void SelectedRule_WhenSetToValidRule_ShouldUpdateRuleInfoProvider()
    {
        // Arrange
        var newRule = typeof(_2ConnectionsRule1);

        // Act
        sut.SelectedRule = newRule;

        // Assert
        ruleInfoProviderMock.VerifySet(x => x.RuleMessage = It.IsAny<string>(), Times.Once);
        ruleInfoProviderMock.VerifySet(x => x.AreRulesBeingApplied = false, Times.Once);
        hintProviderMock.Verify(x => x.ResetSession(), Times.Once);
    }

    [Test]
    public void SelectedRule_WhenSetToNull_ShouldNotUpdateRuleInfoProvider()
    {
        // Arrange & Act
        sut.SelectedRule = null;

        // Assert
        ruleInfoProviderMock.VerifySet(x => x.RuleMessage = It.IsAny<string>(), Times.Never);
        hintProviderMock.Verify(x => x.ResetSession(), Times.Never);
    }

    [Test]
    public void SelectedRule_WhenSetToSameValue_ShouldNotUpdateRuleInfoProvider()
    {
        // Arrange
        var rule = typeof(_1ConnectionRule1);
        sut.SelectedRule = rule;
        ruleInfoProviderMock.Reset();
        hintProviderMock.Reset();

        // Act
        sut.SelectedRule = rule;

        // Assert
        ruleInfoProviderMock.VerifySet(x => x.RuleMessage = It.IsAny<string>(), Times.Never);
        hintProviderMock.Verify(x => x.ResetSession(), Times.Never);
    }

    [Test]
    public void IsTestFieldMode_WhenSet_ShouldUpdateProperty()
    {
        // Arrange & Act
        sut.IsTestFieldMode = true;

        // Assert
        sut.IsTestFieldMode.Should().BeTrue();
    }

    [Test]
    public void IsGeneratingHashiPuzzle_WhenSet_ShouldUpdateProperty()
    {
        // Arrange & Act
        sut.IsGeneratingHashiPuzzle = true;

        // Assert
        sut.IsGeneratingHashiPuzzle.Should().BeTrue();
    }

    [Test]
    public void IsCheating_WhenSet_ShouldUpdateProperty()
    {
        // Arrange & Act
        sut.IsCheating = true;

        // Assert
        sut.IsCheating.Should().BeTrue();
    }

    [Test]
    public void NewRuleName_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newRuleName = "Test Rule Name";

        // Act
        sut.NewRuleName = newRuleName;

        // Assert
        sut.NewRuleName.Should().Be(newRuleName);
    }

    [Test]
    public void HighscoreForSelectedDifficulty_WhenNoHighscoresExist_ShouldReturnNull()
    {
        // Arrange
        settingsMock.Setup(x => x.HighScores).Returns([]);

        // Act
        var highscore = sut.HighscoreForSelectedDifficulty;

        // Assert
        highscore.Should().BeNull();
    }

    [Test]
    public void HighscoreForSelectedDifficulty_WhenMatchingHighscoreExists_ShouldReturnHighscoreTime()
    {
        // Arrange
        var expectedTime = TimeSpan.FromMinutes(5);
        var mockHighScore = new Mock<IHighScorePerDifficultyViewModel>();
        mockHighScore.Setup(x => x.Difficulty).Returns(DifficultyEnum.Easy3);
        mockHighScore.Setup(x => x.HighScoreTime).Returns(expectedTime);

        settingsMock.Setup(x => x.HighScores).Returns([mockHighScore.Object]);
        sut.SelectedDifficulty = DifficultyEnum.Easy3;

        // Act
        var highscore = sut.HighscoreForSelectedDifficulty;

        // Assert
        highscore.Should().Be(expectedTime);
    }

    [Test]
    public void WindowMouseClickedCommand_WhenExecuted_ShouldClearRuleMessage()
    {
        // Arrange & Act
        sut.WindowMouseClickedCommand.Execute(null);

        // Assert
        ruleInfoProviderMock.VerifySet(x => x.RuleMessage = string.Empty, Times.Once);
    }

    [Test]
    public void Receive_WhenCalledWithBridgeConnectionChangedMessage_ShouldNotThrow()
    {
        // Arrange
        var mockMessage = new Mock<IBridgeConnectionChangedMessage>();

        // Act & Assert
        sut.Invoking(x => x.Receive(mockMessage.Object)).Should().NotThrow();
    }

    [Test]
    public void Receive_WhenCalledWithAllConnectionsSetMessage_ShouldNotThrow()
    {
        // Arrange
        var mockMessage = new Mock<IAllConnectionsSetMessage>();

        // Act & Assert
        sut.Invoking(x => x.Receive(mockMessage.Object)).Should().NotThrow();
    }

    [Test]
    public void Receive_WhenCalledWithSetTestSolutionMessage_ShouldNotThrow()
    {
        // Arrange
        var mockMessage = new Mock<ISetTestSolutionMessage>();

        // Act & Assert
        sut.Invoking(x => x.Receive(mockMessage.Object)).Should().NotThrow();
    }

    [Test]
    public void PropertyChanged_ShouldBeRaisedForSelectedDifficulty()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.SelectedDifficulty))
                propertyChangedRaised = true;
        };

        // Act
        sut.SelectedDifficulty = DifficultyEnum.Medium2;

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void PropertyChanged_ShouldBeRaisedForIsTestFieldMode()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.IsTestFieldMode))
                propertyChangedRaised = true;
        };

        // Act
        sut.IsTestFieldMode = true;

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void PropertyChanged_ShouldBeRaisedForTitle_WhenIsTestFieldModeChanges()
    {
        // Arrange
        var titlePropertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.Title))
                titlePropertyChangedRaised = true;
        };

        // Act
        sut.IsTestFieldMode = true;

        // Assert
        titlePropertyChangedRaised.Should().BeTrue();
    }
}