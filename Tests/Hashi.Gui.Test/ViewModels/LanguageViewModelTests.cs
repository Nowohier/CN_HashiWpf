using FluentAssertions;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Test.ViewModels;

[TestFixture]
public class LanguageViewModelTests
{
    private LanguageViewModel languageViewModel;

    [SetUp]
    public void SetUp()
    {
        languageViewModel = new LanguageViewModel("English", "English", "en-GB");
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        var languageNameEnglish = "German";
        var languageNameNative = "Deutsch";
        var culture = "de-DE";

        // Act
        var result = new LanguageViewModel(languageNameEnglish, languageNameNative, culture);

        // Assert
        result.LanguageNameEnglish.Should().Be(languageNameEnglish);
        result.LanguageNameNative.Should().Be(languageNameNative);
        result.Culture.Should().Be(culture);
    }

    [Test]
    public void LanguageNameEnglish_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newLanguageNameEnglish = "French";

        // Act
        languageViewModel.LanguageNameEnglish = newLanguageNameEnglish;

        // Assert
        languageViewModel.LanguageNameEnglish.Should().Be(newLanguageNameEnglish);
    }

    [Test]
    public void LanguageNameNative_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newLanguageNameNative = "Français";

        // Act
        languageViewModel.LanguageNameNative = newLanguageNameNative;

        // Assert
        languageViewModel.LanguageNameNative.Should().Be(newLanguageNameNative);
    }

    [Test]
    public void Culture_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newCulture = "fr-FR";

        // Act
        languageViewModel.Culture = newCulture;

        // Assert
        languageViewModel.Culture.Should().Be(newCulture);
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnLanguageNameEnglish()
    {
        // Arrange
        var expectedLanguageNameEnglish = "Spanish";
        languageViewModel.LanguageNameEnglish = expectedLanguageNameEnglish;

        // Act
        var result = languageViewModel.ToString();

        // Assert
        result.Should().Be(expectedLanguageNameEnglish);
    }

    [Test]
    public void LanguageViewModel_ShouldImplementILanguageViewModel()
    {
        // Act & Assert
        languageViewModel.Should().BeAssignableTo<ILanguageViewModel>();
    }

    [Test]
    public void Constructor_WhenEmptyStrings_ShouldAcceptEmptyStrings()
    {
        // Arrange
        var languageNameEnglish = "";
        var languageNameNative = "";
        var culture = "";

        // Act
        var result = new LanguageViewModel(languageNameEnglish, languageNameNative, culture);

        // Assert
        result.LanguageNameEnglish.Should().Be(languageNameEnglish);
        result.LanguageNameNative.Should().Be(languageNameNative);
        result.Culture.Should().Be(culture);
    }

    [Test]
    public void Constructor_WhenNullParameters_ShouldAcceptNullParameters()
    {
        // Act
        var result = new LanguageViewModel(null!, null!, null!);

        // Assert
        result.LanguageNameEnglish.Should().BeNull();
        result.LanguageNameNative.Should().BeNull();
        result.Culture.Should().BeNull();
    }

    [Test]
    public void Properties_WhenSetToNull_ShouldAcceptNullValues()
    {
        // Act
        languageViewModel.LanguageNameEnglish = null!;
        languageViewModel.LanguageNameNative = null!;
        languageViewModel.Culture = null!;

        // Assert
        languageViewModel.LanguageNameEnglish.Should().BeNull();
        languageViewModel.LanguageNameNative.Should().BeNull();
        languageViewModel.Culture.Should().BeNull();
    }

    [Test]
    public void ToString_WhenLanguageNameEnglishIsNull_ShouldReturnNull()
    {
        // Arrange
        languageViewModel.LanguageNameEnglish = null!;

        // Act
        var result = languageViewModel.ToString();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void ToString_WhenLanguageNameEnglishIsEmpty_ShouldReturnEmptyString()
    {
        // Arrange
        languageViewModel.LanguageNameEnglish = "";

        // Act
        var result = languageViewModel.ToString();

        // Assert
        result.Should().Be("");
    }

    [Test]
    [TestCase("English", "English", "en-GB")]
    [TestCase("German", "Deutsch", "de-DE")]
    [TestCase("French", "Français", "fr-FR")]
    [TestCase("Spanish", "Español", "es-ES")]
    public void Constructor_WhenVariousLanguages_ShouldSetPropertiesCorrectly(
        string languageNameEnglish, string languageNameNative, string culture)
    {
        // Act
        var result = new LanguageViewModel(languageNameEnglish, languageNameNative, culture);

        // Assert
        result.LanguageNameEnglish.Should().Be(languageNameEnglish);
        result.LanguageNameNative.Should().Be(languageNameNative);
        result.Culture.Should().Be(culture);
        result.ToString().Should().Be(languageNameEnglish);
    }
}