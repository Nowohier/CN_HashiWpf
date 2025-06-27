using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows;

namespace Hashi.Gui.Test.Converters;

/// <summary>
/// Unit tests for EmptyStringToVisibilityConverter class.
/// </summary>
public class EmptyStringToVisibilityConverterTests
{
    private EmptyStringToVisibilityConverter sut;

    [SetUp]
    public void SetUp()
    {
        sut = new EmptyStringToVisibilityConverter();
    }

    [Test]
    public void Convert_WhenValueIsNonEmptyString_ShouldReturnVisible()
    {
        // Arrange
        const string value = "test string";

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsEmptyString_ShouldReturnHidden()
    {
        // Arrange
        const string value = "";

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnHidden()
    {
        // Arrange, Act & Assert
        var result = sut.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsWhitespaceString_ShouldReturnVisible()
    {
        // Arrange
        const string value = "   ";

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNonStringObject_ShouldReturnHidden()
    {
        // Arrange
        const int value = 123;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange, Act & Assert
        var action = () => sut.ConvertBack(Visibility.Visible, typeof(string), null, CultureInfo.InvariantCulture);
        action.Should().Throw<NotImplementedException>();
    }
}