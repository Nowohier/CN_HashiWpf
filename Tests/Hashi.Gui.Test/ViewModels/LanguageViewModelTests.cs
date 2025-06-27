using FluentAssertions;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.ViewModels;
using System.ComponentModel;

namespace Hashi.Gui.Test.ViewModels;

/// <summary>
/// Unit tests for LanguageViewModel class.
/// </summary>
public class LanguageViewModelTests
{
    [Test]
    public void Constructor_WhenValidParameters_ShouldSetAllProperties()
    {
        // Arrange
        const string languageNameEnglish = "German";
        const string languageNameNative = "Deutsch";
        const string culture = "de-DE";

        // Act
        var sut = new LanguageViewModel(languageNameEnglish, languageNameNative, culture);

        // Assert
        sut.LanguageNameEnglish.Should().Be(languageNameEnglish);
        sut.LanguageNameNative.Should().Be(languageNameNative);
        sut.Culture.Should().Be(culture);
    }

    [Test]
    public void Constructor_ShouldImplementRequiredInterfaces()
    {
        // Arrange & Act
        var sut = new LanguageViewModel("English", "English", "en-US");

        // Assert
        sut.Should().BeAssignableTo<ILanguageViewModel>();
        sut.Should().BeAssignableTo<INotifyPropertyChanged>();
    }

    [Test]
    public void LanguageNameEnglish_WhenSet_ShouldUpdatePropertyAndRaisePropertyChanged()
    {
        // Arrange
        var sut = new LanguageViewModel("English", "English", "en-US");
        const string newValue = "Spanish";
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LanguageViewModel.LanguageNameEnglish))
                propertyChangedRaised = true;
        };

        // Act
        sut.LanguageNameEnglish = newValue;

        // Assert
        sut.LanguageNameEnglish.Should().Be(newValue);
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void LanguageNameNative_WhenSet_ShouldUpdatePropertyAndRaisePropertyChanged()
    {
        // Arrange
        var sut = new LanguageViewModel("English", "English", "en-US");
        const string newValue = "Español";
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LanguageViewModel.LanguageNameNative))
                propertyChangedRaised = true;
        };

        // Act
        sut.LanguageNameNative = newValue;

        // Assert
        sut.LanguageNameNative.Should().Be(newValue);
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void Culture_WhenSet_ShouldUpdatePropertyAndRaisePropertyChanged()
    {
        // Arrange
        var sut = new LanguageViewModel("English", "English", "en-US");
        const string newValue = "es-ES";
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LanguageViewModel.Culture))
                propertyChangedRaised = true;
        };

        // Act
        sut.Culture = newValue;

        // Assert
        sut.Culture.Should().Be(newValue);
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnLanguageNameEnglish()
    {
        // Arrange
        const string languageNameEnglish = "German";
        var sut = new LanguageViewModel(languageNameEnglish, "Deutsch", "de-DE");

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().Be(languageNameEnglish);
    }

    [Test]
    public void LanguageNameEnglish_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        const string originalValue = "English";
        var sut = new LanguageViewModel(originalValue, "English", "en-US");
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        sut.LanguageNameEnglish = originalValue;

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    [Test]
    public void LanguageNameNative_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        const string originalValue = "English";
        var sut = new LanguageViewModel("English", originalValue, "en-US");
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        sut.LanguageNameNative = originalValue;

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    [Test]
    public void Culture_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        const string originalValue = "en-US";
        var sut = new LanguageViewModel("English", "English", originalValue);
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, _) => propertyChangedRaised = true;

        // Act
        sut.Culture = originalValue;

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }
}