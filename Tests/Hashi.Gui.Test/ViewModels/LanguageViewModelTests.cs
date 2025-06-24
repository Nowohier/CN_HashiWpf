using FluentAssertions;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Test.ViewModels;

[TestFixture]
public class LanguageViewModelTests
{
    private LanguageViewModel sut;
    private const string TestLanguageNameEnglish = "English";
    private const string TestLanguageNameNative = "English";
    private const string TestCulture = "en-GB";

    [SetUp]
    public void SetUp()
    {
        sut = new LanguageViewModel(TestLanguageNameEnglish, TestLanguageNameNative, TestCulture);
    }

    [Test]
    public void Constructor_WhenCalledWithValidParameters_ShouldSetProperties()
    {
        // Arrange & Act
        var result = new LanguageViewModel("English", "English", "en-GB");

        // Assert
        result.LanguageNameEnglish.Should().Be("English");
        result.LanguageNameNative.Should().Be("English");
        result.Culture.Should().Be("en-GB");
    }

    [Test]
    public void Constructor_WhenCalledWithNullParameters_ShouldAcceptNullValues()
    {
        // Arrange & Act
        var result = new LanguageViewModel(null!, null!, null!);

        // Assert
        result.LanguageNameEnglish.Should().BeNull();
        result.LanguageNameNative.Should().BeNull();
        result.Culture.Should().BeNull();
    }

    [Test]
    public void LanguageNameEnglish_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newName = "French";

        // Act
        sut.LanguageNameEnglish = newName;

        // Assert
        sut.LanguageNameEnglish.Should().Be(newName);
    }

    [Test]
    public void LanguageNameEnglish_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.LanguageNameEnglish))
                propertyChangedRaised = true;
        };

        // Act
        sut.LanguageNameEnglish = TestLanguageNameEnglish;

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    [Test]
    public void LanguageNameEnglish_WhenSetToDifferentValue_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.LanguageNameEnglish))
                propertyChangedRaised = true;
        };

        // Act
        sut.LanguageNameEnglish = "Spanish";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void LanguageNameNative_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newName = "Français";

        // Act
        sut.LanguageNameNative = newName;

        // Assert
        sut.LanguageNameNative.Should().Be(newName);
    }

    [Test]
    public void LanguageNameNative_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.LanguageNameNative))
                propertyChangedRaised = true;
        };

        // Act
        sut.LanguageNameNative = TestLanguageNameNative;

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    [Test]
    public void LanguageNameNative_WhenSetToDifferentValue_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.LanguageNameNative))
                propertyChangedRaised = true;
        };

        // Act
        sut.LanguageNameNative = "Español";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void Culture_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newCulture = "fr-FR";

        // Act
        sut.Culture = newCulture;

        // Assert
        sut.Culture.Should().Be(newCulture);
    }

    [Test]
    public void Culture_WhenSetToSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.Culture))
                propertyChangedRaised = true;
        };

        // Act
        sut.Culture = TestCulture;

        // Assert
        propertyChangedRaised.Should().BeFalse();
    }

    [Test]
    public void Culture_WhenSetToDifferentValue_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedRaised = false;
        sut.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(sut.Culture))
                propertyChangedRaised = true;
        };

        // Act
        sut.Culture = "es-ES";

        // Assert
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnLanguageNameEnglish()
    {
        // Arrange & Act
        var result = sut.ToString();

        // Assert
        result.Should().Be(TestLanguageNameEnglish);
    }

    [Test]
    public void ToString_WhenLanguageNameEnglishIsNull_ShouldReturnNull()
    {
        // Arrange
        sut.LanguageNameEnglish = null!;

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void ToString_WhenLanguageNameEnglishIsEmpty_ShouldReturnEmpty()
    {
        // Arrange
        sut.LanguageNameEnglish = string.Empty;

        // Act
        var result = sut.ToString();

        // Assert
        result.Should().Be(string.Empty);
    }

    [Test]
    public void Properties_WhenSetToNull_ShouldAcceptNullValues()
    {
        // Arrange & Act
        sut.LanguageNameEnglish = null!;
        sut.LanguageNameNative = null!;
        sut.Culture = null!;

        // Assert
        sut.LanguageNameEnglish.Should().BeNull();
        sut.LanguageNameNative.Should().BeNull();
        sut.Culture.Should().BeNull();
    }

    [Test]
    public void Properties_WhenSetToEmptyString_ShouldAcceptEmptyValues()
    {
        // Arrange & Act
        sut.LanguageNameEnglish = string.Empty;
        sut.LanguageNameNative = string.Empty;
        sut.Culture = string.Empty;

        // Assert
        sut.LanguageNameEnglish.Should().Be(string.Empty);
        sut.LanguageNameNative.Should().Be(string.Empty);
        sut.Culture.Should().Be(string.Empty);
    }

    [Test]
    public void PropertyChanged_ShouldBeRaisedOnce_WhenPropertyChanges()
    {
        // Arrange
        var propertyChangedCount = 0;
        sut.PropertyChanged += (sender, args) => propertyChangedCount++;

        // Act
        sut.LanguageNameEnglish = "New Value";
        sut.LanguageNameNative = "New Native Value";
        sut.Culture = "new-CU";

        // Assert
        propertyChangedCount.Should().Be(3);
    }
}