using FluentAssertions;
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

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalledWithValidParameters_ShouldInitializeProperties()
    {
        // Arrange & Act
        var result = new LanguageViewModel("German", "Deutsch", "de-DE");

        // Assert
        result.LanguageNameEnglish.Should().Be("German");
        result.LanguageNameNative.Should().Be("Deutsch");
        result.Culture.Should().Be("de-DE");
    }

    [Test]
    public void Constructor_WhenCalledWithNullValues_ShouldAcceptNullValues()
    {
        // Arrange & Act
        var result = new LanguageViewModel(null!, null!, null!);

        // Assert
        result.LanguageNameEnglish.Should().BeNull();
        result.LanguageNameNative.Should().BeNull();
        result.Culture.Should().BeNull();
    }

    [Test]
    public void Constructor_WhenCalledWithEmptyStrings_ShouldAcceptEmptyStrings()
    {
        // Arrange & Act
        var result = new LanguageViewModel("", "", "");

        // Assert
        result.LanguageNameEnglish.Should().Be("");
        result.LanguageNameNative.Should().Be("");
        result.Culture.Should().Be("");
    }

    #endregion

    #region Property Tests

    [Test]
    public void LanguageNameEnglish_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newValue = "French";

        // Act
        languageViewModel.LanguageNameEnglish = newValue;

        // Assert
        languageViewModel.LanguageNameEnglish.Should().Be(newValue);
    }

    [Test]
    public void LanguageNameEnglish_WhenSetToNull_ShouldAcceptNull()
    {
        // Act
        languageViewModel.LanguageNameEnglish = null!;

        // Assert
        languageViewModel.LanguageNameEnglish.Should().BeNull();
    }

    [Test]
    public void LanguageNameNative_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newValue = "Français";

        // Act
        languageViewModel.LanguageNameNative = newValue;

        // Assert
        languageViewModel.LanguageNameNative.Should().Be(newValue);
    }

    [Test]
    public void LanguageNameNative_WhenSetToNull_ShouldAcceptNull()
    {
        // Act
        languageViewModel.LanguageNameNative = null!;

        // Assert
        languageViewModel.LanguageNameNative.Should().BeNull();
    }

    [Test]
    public void Culture_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newValue = "fr-FR";

        // Act
        languageViewModel.Culture = newValue;

        // Assert
        languageViewModel.Culture.Should().Be(newValue);
    }

    [Test]
    public void Culture_WhenSetToNull_ShouldAcceptNull()
    {
        // Act
        languageViewModel.Culture = null!;

        // Assert
        languageViewModel.Culture.Should().BeNull();
    }

    #endregion

    #region Method Tests

    [Test]
    public void ToString_WhenCalled_ShouldReturnLanguageNameEnglish()
    {
        // Act
        var result = languageViewModel.ToString();

        // Assert
        result.Should().Be("English");
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
    public void ToString_WhenLanguageNameEnglishHasWhitespace_ShouldReturnWhitespace()
    {
        // Arrange
        languageViewModel.LanguageNameEnglish = "   ";

        // Act
        var result = languageViewModel.ToString();

        // Assert
        result.Should().Be("   ");
    }

    #endregion

    #region Property Change Notification Tests

    [Test]
    public void LanguageNameEnglish_WhenChanged_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        languageViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LanguageViewModel.LanguageNameEnglish))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        languageViewModel.LanguageNameEnglish = "New Value";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void LanguageNameNative_WhenChanged_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        languageViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LanguageViewModel.LanguageNameNative))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        languageViewModel.LanguageNameNative = "New Value";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void Culture_WhenChanged_ShouldRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        languageViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LanguageViewModel.Culture))
            {
                propertyChangedRaised = true;
            }
        };

        // Act
        languageViewModel.Culture = "New Value";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void Properties_WhenSetToSameValue_ShouldNotRaisePropertyChangedEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        languageViewModel.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        languageViewModel.LanguageNameEnglish = "English"; // Same as initial value
        languageViewModel.LanguageNameNative = "English"; // Same as initial value
        languageViewModel.Culture = "en-GB"; // Same as initial value

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    #endregion
}