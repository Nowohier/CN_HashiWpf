using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels.Settings;

namespace Hashi.Gui.Test.ViewModels.Settings;

[TestFixture]
public class SettingsViewModelTests
{
    private SettingsViewModel settingsViewModel;

    [SetUp]
    public void SetUp()
    {
        settingsViewModel = new SettingsViewModel();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeProperties()
    {
        // Act
        var result = new SettingsViewModel();

        // Assert
        result.AreGridLinesEnabled.Should().BeFalse();
        result.HighScores.Should().NotBeNull();
        result.HighScores.Should().BeEmpty();
        result.Languages.Should().NotBeNull();
        result.Languages.Should().HaveCount(2);
        result.Languages.Should().Contain(l => l.Culture == "en-GB");
        result.Languages.Should().Contain(l => l.Culture == "de-DE");
        result.SelectedLanguageCulture.Should().Be("en-GB");
    }

    [Test]
    public void AreGridLinesEnabled_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newValue = true;

        // Act
        settingsViewModel.AreGridLinesEnabled = newValue;

        // Assert
        settingsViewModel.AreGridLinesEnabled.Should().Be(newValue);
    }

    [Test]
    public void SelectedLanguageCulture_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newCulture = "de-DE";

        // Act
        settingsViewModel.SelectedLanguageCulture = newCulture;

        // Assert
        settingsViewModel.SelectedLanguageCulture.Should().Be(newCulture);
    }

    [Test]
    public void SelectedLanguageCulture_WhenSetToNull_ShouldReturnFirstLanguageCulture()
    {
        // Act
        settingsViewModel.SelectedLanguageCulture = null;

        // Assert
        settingsViewModel.SelectedLanguageCulture.Should().Be("en-GB");
    }

    [Test]
    public void InitializeHighScores_WhenCalled_ShouldPopulateHighScoresWithAllDifficulties()
    {
        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        settingsViewModel.HighScores.Should().HaveCount(Enum.GetValues<DifficultyEnum>().Length);
        settingsViewModel.HighScores.Should().Contain(h => h.Difficulty == DifficultyEnum.Easy);
        settingsViewModel.HighScores.Should().Contain(h => h.Difficulty == DifficultyEnum.Medium);
        settingsViewModel.HighScores.Should().Contain(h => h.Difficulty == DifficultyEnum.Hard);
    }

    [Test]
    public void InitializeHighScores_WhenCalledMultipleTimes_ShouldClearAndRepopulateHighScores()
    {
        // Arrange
        settingsViewModel.InitializeHighScores();
        var initialCount = settingsViewModel.HighScores.Count;

        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        settingsViewModel.HighScores.Should().HaveCount(initialCount);
        settingsViewModel.HighScores.Should().HaveCount(Enum.GetValues<DifficultyEnum>().Length);
    }

    [Test]
    public void Languages_WhenInitialized_ShouldContainExpectedLanguages()
    {
        // Act
        var englishLanguage = settingsViewModel.Languages.FirstOrDefault(l => l.Culture == "en-GB");
        var germanLanguage = settingsViewModel.Languages.FirstOrDefault(l => l.Culture == "de-DE");

        // Assert
        englishLanguage.Should().NotBeNull();
        englishLanguage!.LanguageNameEnglish.Should().Be("English");
        englishLanguage.LanguageNameNative.Should().Be("English");

        germanLanguage.Should().NotBeNull();
        germanLanguage!.LanguageNameEnglish.Should().Be("German");
        germanLanguage.LanguageNameNative.Should().Be("Deutsch");
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnFormattedString()
    {
        // Arrange
        settingsViewModel.InitializeHighScores();

        // Act
        var result = settingsViewModel.ToString();

        // Assert
        result.Should().StartWith("HighScores:");
        result.Should().Contain("Difficulty:");
    }

    [Test]
    public void ToString_WhenHighScoresIsEmpty_ShouldReturnEmptyHighScoresString()
    {
        // Act
        var result = settingsViewModel.ToString();

        // Assert
        result.Should().Be("HighScores: ");
    }

    [Test]
    public void SettingsViewModel_ShouldImplementISettingsViewModel()
    {
        // Act & Assert
        settingsViewModel.Should().BeAssignableTo<ISettingsViewModel>();
    }

    [Test]
    public void HighScores_WhenAccessed_ShouldReturnObservableCollection()
    {
        // Act & Assert
        settingsViewModel.HighScores.Should().NotBeNull();
        settingsViewModel.HighScores.Should().BeAssignableTo<System.Collections.ObjectModel.ObservableCollection<IHighScorePerDifficultyViewModel>>();
    }

    [Test]
    public void Languages_WhenAccessed_ShouldReturnObservableCollection()
    {
        // Act & Assert
        settingsViewModel.Languages.Should().NotBeNull();
        settingsViewModel.Languages.Should().BeAssignableTo<System.Collections.ObjectModel.ObservableCollection<ILanguageViewModel>>();
    }

    [Test]
    public void SelectedLanguageCulture_WhenLanguagesIsEmpty_ShouldReturnNull()
    {
        // Arrange
        var emptyViewModel = new SettingsViewModel();
        emptyViewModel.Languages.Clear();

        // Act
        var result = emptyViewModel.SelectedLanguageCulture;

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void InitializeHighScores_WhenCalledAfterManualAdd_ShouldReplaceExistingHighScores()
    {
        // Arrange
        var mockHighScore = new HighScorePerDifficultyViewModel(DifficultyEnum.Easy);
        settingsViewModel.HighScores.Add(mockHighScore);

        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        settingsViewModel.HighScores.Should().NotContain(mockHighScore);
        settingsViewModel.HighScores.Should().HaveCount(Enum.GetValues<DifficultyEnum>().Length);
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldAlwaysInitializeLanguagesCorrectly()
    {
        // Act
        var viewModel1 = new SettingsViewModel();
        var viewModel2 = new SettingsViewModel();

        // Assert
        viewModel1.Languages.Should().HaveCount(2);
        viewModel2.Languages.Should().HaveCount(2);
        viewModel1.Languages.Should().NotBeSameAs(viewModel2.Languages);
    }

    [Test]
    public void AreGridLinesEnabled_WhenToggledMultipleTimes_ShouldMaintainCorrectState()
    {
        // Arrange
        var initialValue = settingsViewModel.AreGridLinesEnabled;

        // Act & Assert
        settingsViewModel.AreGridLinesEnabled = !initialValue;
        settingsViewModel.AreGridLinesEnabled.Should().Be(!initialValue);

        settingsViewModel.AreGridLinesEnabled = initialValue;
        settingsViewModel.AreGridLinesEnabled.Should().Be(initialValue);
    }

    [Test]
    public void SelectedLanguageCulture_WhenSetToValidCulture_ShouldUpdateCorrectly()
    {
        // Arrange
        var cultures = new[] { "en-GB", "de-DE", "fr-FR", "es-ES" };

        // Act & Assert
        foreach (var culture in cultures)
        {
            settingsViewModel.SelectedLanguageCulture = culture;
            settingsViewModel.SelectedLanguageCulture.Should().Be(culture);
        }
    }
}