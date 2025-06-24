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
    private Mock<Func<SolidColorBrush, IHashiBrush>> mockBrushFactory;
    private Mock<IDialogWrapper> mockDialogWrapper;
    private Mock<IHashiGenerator> mockHashiGenerator;
    private Mock<ISettingsProvider> mockSettingsProvider;
    private Mock<ITimerProvider> mockTimerProvider;
    private Mock<IIslandProvider> mockIslandProvider;
    private Mock<IHintProvider> mockHintProvider;
    private Mock<ITestSolutionProvider> mockTestSolutionProvider;
    private Mock<IResourceManager> mockResourceManager;
    private Mock<ILoggerFactory> mockLoggerFactory;
    private Mock<ILogger> mockLogger;
    private Mock<ISettingsViewModel> mockSettings;
    private Mock<IRuleInfoProvider> mockRuleInfoProvider;
    private MainViewModel sut;

    [SetUp]
    public void SetUp()
    {
        mockBrushFactory = new Mock<Func<SolidColorBrush, IHashiBrush>>();
        mockDialogWrapper = new Mock<IDialogWrapper>();
        mockHashiGenerator = new Mock<IHashiGenerator>();
        mockSettingsProvider = new Mock<ISettingsProvider>();
        mockTimerProvider = new Mock<ITimerProvider>();
        mockIslandProvider = new Mock<IIslandProvider>();
        mockHintProvider = new Mock<IHintProvider>();
        mockTestSolutionProvider = new Mock<ITestSolutionProvider>();
        mockResourceManager = new Mock<IResourceManager>();
        mockLoggerFactory = new Mock<ILoggerFactory>();
        mockLogger = new Mock<ILogger>();
        mockSettings = new Mock<ISettingsViewModel>();
        mockRuleInfoProvider = new Mock<IRuleInfoProvider>();

        mockLoggerFactory.Setup(x => x.CreateLogger<MainViewModel>()).Returns(mockLogger.Object);
        mockSettingsProvider.Setup(x => x.Settings).Returns(mockSettings.Object);
        mockHintProvider.Setup(x => x.RuleInfoProvider).Returns(mockRuleInfoProvider.Object);
        mockHintProvider.Setup(x => x.Rules).Returns(new List<Type> { typeof(_1ConnectionRule1), typeof(_2ConnectionsRule1) });
        mockSettings.Setup(x => x.HighScores).Returns(new List<IHighScorePerDifficultyViewModel>());

        var mockHashiBrush = new Mock<IHashiBrush>();
        mockBrushFactory.Setup(x => x.Invoke(It.IsAny<SolidColorBrush>())).Returns(mockHashiBrush.Object);

        sut = new MainViewModel(
            mockBrushFactory.Object,
            mockDialogWrapper.Object,
            mockHashiGenerator.Object,
            mockSettingsProvider.Object,
            mockTimerProvider.Object,
            mockIslandProvider.Object,
            mockHintProvider.Object,
            mockTestSolutionProvider.Object,
            mockResourceManager.Object,
            mockLoggerFactory.Object);
    }

    [Test]
    public void Constructor_WhenCalledWithValidDependencies_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new MainViewModel(
            mockBrushFactory.Object,
            mockDialogWrapper.Object,
            mockHashiGenerator.Object,
            mockSettingsProvider.Object,
            mockTimerProvider.Object,
            mockIslandProvider.Object,
            mockHintProvider.Object,
            mockTestSolutionProvider.Object,
            mockResourceManager.Object,
            mockLoggerFactory.Object);

        // Assert
        result.SettingsProvider.Should().Be(mockSettingsProvider.Object);
        result.TimerProvider.Should().Be(mockTimerProvider.Object);
        result.IslandProvider.Should().Be(mockIslandProvider.Object);
        result.HintProvider.Should().Be(mockHintProvider.Object);
        result.TestSolutionProvider.Should().Be(mockTestSolutionProvider.Object);
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
        mockResourceManager.Verify(x => x.PrepareUi(), Times.Once);
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
        mockBrushFactory.Verify(x => x.Invoke(It.IsAny<SolidColorBrush>()), Times.Once);
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
        var newDifficulty = DifficultyEnum.Hard15;

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
        mockRuleInfoProvider.VerifySet(x => x.RuleMessage = It.IsAny<string>(), Times.Once);
        mockRuleInfoProvider.VerifySet(x => x.AreRulesBeingApplied = false, Times.Once);
        mockHintProvider.Verify(x => x.ResetSession(), Times.Once);
    }

    [Test]
    public void SelectedRule_WhenSetToNull_ShouldNotUpdateRuleInfoProvider()
    {
        // Arrange & Act
        sut.SelectedRule = null;

        // Assert
        mockRuleInfoProvider.VerifySet(x => x.RuleMessage = It.IsAny<string>(), Times.Never);
        mockHintProvider.Verify(x => x.ResetSession(), Times.Never);
    }

    [Test]
    public void SelectedRule_WhenSetToSameValue_ShouldNotUpdateRuleInfoProvider()
    {
        // Arrange
        var rule = typeof(_1ConnectionRule1);
        sut.SelectedRule = rule;
        mockRuleInfoProvider.Reset();
        mockHintProvider.Reset();

        // Act
        sut.SelectedRule = rule;

        // Assert
        mockRuleInfoProvider.VerifySet(x => x.RuleMessage = It.IsAny<string>(), Times.Never);
        mockHintProvider.Verify(x => x.ResetSession(), Times.Never);
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
        mockSettings.Setup(x => x.HighScores).Returns(new List<IHighScorePerDifficultyViewModel>());

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
        
        mockSettings.Setup(x => x.HighScores).Returns(new List<IHighScorePerDifficultyViewModel> { mockHighScore.Object });
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
        mockRuleInfoProvider.VerifySet(x => x.RuleMessage = string.Empty, Times.Once);
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
        sut.SelectedDifficulty = DifficultyEnum.Medium7;

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