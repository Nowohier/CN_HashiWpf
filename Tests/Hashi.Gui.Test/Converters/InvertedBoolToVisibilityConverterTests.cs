using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows;

namespace Hashi.Gui.Test.Converters;

/// <summary>
/// Unit tests for InvertedBoolToVisibilityConverter class.
/// </summary>
public class InvertedBoolToVisibilityConverterTests
{
    private InvertedBoolToVisibilityConverter sut;

    [SetUp]
    public void SetUp()
    {
        sut = new InvertedBoolToVisibilityConverter();
    }

    [Test]
    public void Convert_WhenValueIsTrue_ShouldReturnCollapsed()
    {
        // Arrange
        const bool value = true;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsFalse_ShouldReturnVisible()
    {
        // Arrange
        const bool value = false;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnVisible()
    {
        // Arrange, Act & Assert
        var result = sut.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNonBooleanObject_ShouldThrowInvalidCastException()
    {
        // Arrange
        const string value = "test";

        // Act & Assert
        var action = () => sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        action.Should().Throw<InvalidCastException>();
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange, Act & Assert
        var action = () => sut.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture);
        action.Should().Throw<NotImplementedException>();
    }
}