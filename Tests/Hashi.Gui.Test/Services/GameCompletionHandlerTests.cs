using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Services;
using Moq;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Test.Services;

[TestFixture]
public class GameCompletionHandlerTests
{
    private Mock<IDialogWrapper> dialogWrapperMock;
    private Mock<ISettingsProvider> settingsProviderMock;
    private GameCompletionHandler handler;

    [SetUp]
    public void SetUp()
    {
        dialogWrapperMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        settingsProviderMock = new Mock<ISettingsProvider>(MockBehavior.Strict);

        var settingsMock = new Mock<ISettingsViewModel>(MockBehavior.Strict);
        var highScores = new ObservableCollection<IHighScorePerDifficultyViewModel>();
        var highScoreMock = new Mock<IHighScorePerDifficultyViewModel>(MockBehavior.Strict);
        highScoreMock.Setup(x => x.Difficulty).Returns(DifficultyEnum.Easy3);
        highScoreMock.SetupProperty(x => x.HighScoreTime);
        highScores.Add(highScoreMock.Object);

        settingsMock.Setup(x => x.HighScores).Returns(highScores);
        settingsProviderMock.Setup(x => x.Settings).Returns(settingsMock.Object);
        settingsProviderMock.Setup(x => x.SaveSettings());

        handler = new GameCompletionHandler(dialogWrapperMock.Object, settingsProviderMock.Object);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenDialogWrapperIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new GameCompletionHandler(null!, settingsProviderMock.Object);
        action.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenSettingsProviderIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new GameCompletionHandler(dialogWrapperMock.Object, null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region HandleGameCompletion Tests

    [Test]
    public void HandleGameCompletion_WhenCheating_ShouldShowDialogAndReturnTrue()
    {
        // Arrange
        dialogWrapperMock.Setup(x =>
                x.Show(It.IsAny<string>(), It.IsAny<string>(), DialogButton.Ok, DialogImage.Success))
            .Returns(DialogResult.Ok);

        // Act
        var result = handler.HandleGameCompletion(TimeSpan.FromMinutes(5), DifficultyEnum.Easy3, true, false);

        // Assert
        result.Should().BeTrue();
        dialogWrapperMock.Verify(
            x => x.Show(It.IsAny<string>(), It.IsAny<string>(), DialogButton.Ok, DialogImage.Success), Times.Once);
    }

    [Test]
    public void HandleGameCompletion_WhenInTestFieldMode_ShouldShowDialogAndReturnFalse()
    {
        // Arrange
        dialogWrapperMock.Setup(x =>
                x.Show(It.IsAny<string>(), It.IsAny<string>(), DialogButton.Ok, DialogImage.Success))
            .Returns(DialogResult.Ok);

        // Act
        var result = handler.HandleGameCompletion(TimeSpan.FromMinutes(5), DifficultyEnum.Easy3, false, true);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void HandleGameCompletion_WhenNormalGameNoHighScore_ShouldReturnTrue()
    {
        // Arrange
        dialogWrapperMock.Setup(x =>
                x.Show(It.IsAny<string>(), It.IsAny<string>(), DialogButton.Ok, DialogImage.Success))
            .Returns(DialogResult.Ok);

        // Act
        var result = handler.HandleGameCompletion(TimeSpan.FromMinutes(5), DifficultyEnum.Easy1, false, false);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
