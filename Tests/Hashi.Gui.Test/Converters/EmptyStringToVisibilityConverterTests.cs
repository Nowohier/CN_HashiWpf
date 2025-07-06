using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class EmptyStringToVisibilityConverterTests
{
    private EmptyStringToVisibilityConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new EmptyStringToVisibilityConverter();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new EmptyStringToVisibilityConverter();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsEmptyString_ShouldReturnHidden()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNonEmptyString_ShouldReturnVisible()
    {
        // Arrange
        var value = "Test string";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnHidden()
    {
        // Act
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNotString_ShouldReturnHidden()
    {
        // Arrange
        var value = 123;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsWhitespaceString_ShouldReturnHidden()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsStringWithContent_ShouldReturnVisible()
    {
        // Arrange
        var value = "Some content";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsStringWithOnlySpaces_ShouldReturnHidden()
    {
        // Arrange
        var value = "     ";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsStringWithTabsAndSpaces_ShouldReturnHidden()
    {
        // Arrange
        var value = " \t \n ";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsSingleCharacter_ShouldReturnVisible()
    {
        // Arrange
        var value = "a";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsZeroLengthString_ShouldReturnHidden()
    {
        // Arrange
        var value = "";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var act = () => converter.ConvertBack(Visibility.Visible, typeof(string), null, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    [TestCase("Hello World")]
    [TestCase("123")]
    [TestCase("Special@Characters#")]
    [TestCase("Multi\nLine\nString")]
    public void Convert_WhenValueIsNonEmptyString_ShouldAlwaysReturnVisible(string testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("\t")]
    [TestCase("\n")]
    [TestCase("\r\n")]
    [TestCase(" \t \n \r ")]
    public void Convert_WhenValueIsNullOrWhitespace_ShouldAlwaysReturnHidden(string? testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    [TestCase(123)]
    [TestCase(true)]
    [TestCase(45.67)]
    [TestCase(new object())]
    public void Convert_WhenValueIsNotString_ShouldAlwaysReturnHidden(object testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenParameterAndCultureProvided_ShouldIgnoreThemAndWorkCorrectly()
    {
        // Arrange
        var value = "Test";
        var parameter = "SomeParameter";
        var culture = new CultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, typeof(Visibility), parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }
}