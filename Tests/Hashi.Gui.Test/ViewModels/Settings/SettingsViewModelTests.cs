using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels;
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

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeCollections()
    {
        // Arrange & Act
        var result = new SettingsViewModel();

        // Assert
        result.HighScores.Should().NotBeNull();
        result.Languages.Should().NotBeNull();
        result.Languages.Should().HaveCount(2); // English and German
        result.AreGridLinesEnabled.Should().BeFalse(); // Default value
    }

    [Test]
    public void Constructor_WhenCalled_ShouldInitializeLanguagesCorrectly()
    {
        // Arrange & Act
        var result = new SettingsViewModel();

        // Assert
        result.Languages.Should().HaveCount(2);

        var englishLanguage = result.Languages.FirstOrDefault(l => l.Culture == "en-GB");
        var germanLanguage = result.Languages.FirstOrDefault(l => l.Culture == "de-DE");

        englishLanguage.Should().NotBeNull();
        englishLanguage!.LanguageNameEnglish.Should().Be("English");
        englishLanguage.LanguageNameNative.Should().Be("English");

        germanLanguage.Should().NotBeNull();
        germanLanguage!.LanguageNameEnglish.Should().Be("German");
        germanLanguage.LanguageNameNative.Should().Be("Deutsch");
    }

    #endregion

    #region Property Tests

    [Test]
    public void AreGridLinesEnabled_WhenSet_ShouldUpdateProperty()
    {
        // Act
        settingsViewModel.AreGridLinesEnabled = true;

        // Assert
        settingsViewModel.AreGridLinesEnabled.Should().BeTrue();
    }

    [Test]
    public void AreGridLinesEnabled_WhenToggled_ShouldUpdateProperty()
    {
        // Arrange
        var initialValue = settingsViewModel.AreGridLinesEnabled;

        // Act
        settingsViewModel.AreGridLinesEnabled = !initialValue;

        // Assert
        settingsViewModel.AreGridLinesEnabled.Should().Be(!initialValue);
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
    public void SelectedLanguageCulture_WhenNotSet_ShouldReturnFirstLanguageCulture()
    {
        // Arrange
        var newSettingsViewModel = new SettingsViewModel();

        // Act
        var result = newSettingsViewModel.SelectedLanguageCulture;

        // Assert
        result.Should().Be("en-GB"); // First language is English
    }

    [Test]
    public void SelectedLanguageCulture_WhenNoLanguagesExist_ShouldReturnNull()
    {
        // Arrange
        var newSettingsViewModel = new SettingsViewModel();
        newSettingsViewModel.Languages.Clear();

        // Act
        var result = newSettingsViewModel.SelectedLanguageCulture;

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void SelectedLanguageCulture_WhenSetToNull_ShouldAcceptNull()
    {
        // Act
        settingsViewModel.SelectedLanguageCulture = null;

        // Assert
        settingsViewModel.SelectedLanguageCulture.Should().Be("en-GB"); // Falls back to first language
    }

    [Test]
    public void HighScores_ShouldBeInitializedAsEmptyCollection()
    {
        // Act & Assert
        settingsViewModel.HighScores.Should().BeEmpty();
        settingsViewModel.HighScores.Should().NotBeNull();
    }

    [Test]
    public void Languages_ShouldBeInitializedWithTwoLanguages()
    {
        // Act & Assert
        settingsViewModel.Languages.Should().HaveCount(2);
        settingsViewModel.Languages.Should().NotBeNull();
    }

    #endregion

    #region InitializeHighScores Tests

    [Test]
    public void InitializeHighScores_WhenCalled_ShouldAddAllDifficultyEnums()
    {
        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        var expectedDifficulties = Enum.GetValues<DifficultyEnum>();
        settingsViewModel.HighScores.Should().HaveCount(expectedDifficulties.Length);

        foreach (var difficulty in expectedDifficulties)
        {
            settingsViewModel.HighScores.Should().Contain(hs => hs.Difficulty == difficulty);
        }
    }

    [Test]
    public void InitializeHighScores_WhenCalledMultipleTimes_ShouldReplaceExistingHighScores()
    {
        // Arrange
        settingsViewModel.InitializeHighScores();
        var initialCount = settingsViewModel.HighScores.Count;

        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        settingsViewModel.HighScores.Should().HaveCount(initialCount);
    }

    [Test]
    public void InitializeHighScores_WhenCalledWithExistingHighScores_ShouldClearAndReplace()
    {
        // Arrange
        settingsViewModel.HighScores.Add(new HighScorePerDifficultyViewModel(DifficultyEnum.Easy2)
        {
            HighScoreTime = TimeSpan.FromMinutes(5)
        });

        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        var easyHighScore = settingsViewModel.HighScores.FirstOrDefault(hs => hs.Difficulty == DifficultyEnum.Easy2);
        easyHighScore.Should().NotBeNull();
        easyHighScore!.HighScoreTime.Should().BeNull(); // Should be a new instance
    }

    [Test]
    public void InitializeHighScores_WhenCalled_ShouldCreateHighScorePerDifficultyViewModels()
    {
        // Act
        settingsViewModel.InitializeHighScores();

        // Assert
        settingsViewModel.HighScores.Should().AllBeOfType<HighScorePerDifficultyViewModel>();

        foreach (var highScore in settingsViewModel.HighScores)
        {
            highScore.Should().BeAssignableTo<IHighScorePerDifficultyViewModel>();
            highScore.HighScoreTime.Should().BeNull(); // Default value
        }
    }

    #endregion

    #region ToString Tests

    [Test]
    public void ToString_WhenHighScoresIsEmpty_ShouldReturnStringWithEmptyHighScores()
    {
        // Act
        var result = settingsViewModel.ToString();

        // Assert
        result.Should().Contain("HighScores:");
        result.Should().NotContain("Difficulty:");
    }

    [Test]
    public void ToString_WhenHighScoresHasItems_ShouldReturnFormattedString()
    {
        // Arrange
        settingsViewModel.InitializeHighScores();

        // Act
        var result = settingsViewModel.ToString();

        // Assert
        result.Should().Contain("HighScores:");
        result.Should().Contain("Difficulty:");
        result.Should().Contain("HighScoreTime:");
        result.Should().Contain(" | ");
    }

    [Test]
    public void ToString_WhenCalledMultipleTimes_ShouldReturnConsistentResults()
    {
        // Arrange
        settingsViewModel.InitializeHighScores();

        // Act
        var result1 = settingsViewModel.ToString();
        var result2 = settingsViewModel.ToString();

        // Assert
        result1.Should().Be(result2);
    }

    #endregion

    #region Property Change Notification Tests

    [Test]
    public void AreGridLinesEnabled_WhenChanged_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        settingsViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SettingsViewModel.AreGridLinesEnabled))
                propertyChangedRaised = true;
        };

        // Act
        settingsViewModel.AreGridLinesEnabled = true;

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void SelectedLanguageCulture_WhenChanged_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        settingsViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SettingsViewModel.SelectedLanguageCulture))
                propertyChangedRaised = true;
        };

        // Act
        settingsViewModel.SelectedLanguageCulture = "de-DE";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void Properties_WhenSetToSameValue_ShouldNotRaisePropertyChangedEvent()
    {
        // Arrange
        settingsViewModel.AreGridLinesEnabled = false;
        settingsViewModel.SelectedLanguageCulture = "en-GB";

        var propertyChangedRaised = false;
        settingsViewModel.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        settingsViewModel.AreGridLinesEnabled = false; // Same value
        settingsViewModel.SelectedLanguageCulture = "en-GB"; // Same value

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    #endregion

    #region Collection Manipulation Tests

    [Test]
    public void HighScores_WhenItemsAddedManually_ShouldAcceptNewItems()
    {
        // Arrange
        var newHighScore = new HighScorePerDifficultyViewModel(DifficultyEnum.Easy2);

        // Act
        settingsViewModel.HighScores.Add(newHighScore);

        // Assert
        settingsViewModel.HighScores.Should().Contain(newHighScore);
        settingsViewModel.HighScores.Should().HaveCount(1);
    }

    [Test]
    public void HighScores_WhenItemsRemovedManually_ShouldRemoveItems()
    {
        // Arrange
        var highScore = new HighScorePerDifficultyViewModel(DifficultyEnum.Easy2);
        settingsViewModel.HighScores.Add(highScore);

        // Act
        settingsViewModel.HighScores.Remove(highScore);

        // Assert
        settingsViewModel.HighScores.Should().NotContain(highScore);
        settingsViewModel.HighScores.Should().BeEmpty();
    }

    [Test]
    public void Languages_WhenItemsAddedManually_ShouldAcceptNewItems()
    {
        // Arrange
        var newLanguage = new LanguageViewModel("French", "Français", "fr-FR");

        // Act
        settingsViewModel.Languages.Add(newLanguage);

        // Assert
        settingsViewModel.Languages.Should().Contain(newLanguage);
        settingsViewModel.Languages.Should().HaveCount(3); // 2 initial + 1 new
    }

    [Test]
    public void Languages_WhenItemsRemovedManually_ShouldRemoveItems()
    {
        // Arrange
        var germanLanguage = settingsViewModel.Languages.First(l => l.Culture == "de-DE");

        // Act
        settingsViewModel.Languages.Remove(germanLanguage);

        // Assert
        settingsViewModel.Languages.Should().NotContain(germanLanguage);
        settingsViewModel.Languages.Should().HaveCount(1);
    }

    #endregion

    #region JSON Serialization Tests

    [Test]
    public void JsonProperties_ShouldBeCorrectlyDecorated()
    {
        // Arrange & Act
        var areGridLinesEnabledProperty = typeof(SettingsViewModel).GetProperty(nameof(SettingsViewModel.AreGridLinesEnabled));
        var selectedLanguageCultureProperty = typeof(SettingsViewModel).GetProperty(nameof(SettingsViewModel.SelectedLanguageCulture));
        var highScoresProperty = typeof(SettingsViewModel).GetProperty(nameof(SettingsViewModel.HighScores));

        // Assert
        areGridLinesEnabledProperty.Should().NotBeNull();
        selectedLanguageCultureProperty.Should().NotBeNull();
        highScoresProperty.Should().NotBeNull();

        // Verify JSON attributes exist
        var areGridLinesEnabledHasJsonProperty = areGridLinesEnabledProperty!.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).Any();
        var selectedLanguageCultureHasJsonProperty = selectedLanguageCultureProperty!.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).Any();
        var highScoresHasJsonProperty = highScoresProperty!.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).Any();

        areGridLinesEnabledHasJsonProperty.Should().BeTrue();
        selectedLanguageCultureHasJsonProperty.Should().BeTrue();
        highScoresHasJsonProperty.Should().BeTrue();
    }

    [Test]
    public void Class_ShouldHaveJsonObjectAttribute()
    {
        // Arrange & Act
        var jsonObjectAttribute = typeof(SettingsViewModel).GetCustomAttributes(typeof(Newtonsoft.Json.JsonObjectAttribute), false);

        // Assert
        jsonObjectAttribute.Should().NotBeEmpty();
    }

    #endregion
}