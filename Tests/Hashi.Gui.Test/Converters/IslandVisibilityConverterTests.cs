using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows;

namespace Hashi.Gui.Test.Converters;

/// <summary>
/// Unit tests for IslandVisibilityConverter class.
/// </summary>
public class IslandVisibilityConverterTests
{
    private IslandVisibilityConverter sut;

    [SetUp]
    public void SetUp()
    {
        sut = new IslandVisibilityConverter();
    }

    [Test]
    public void Convert_WhenValueIsPositiveInteger_ShouldReturnVisible()
    {
        // Arrange
        const int value = 3;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsZero_ShouldReturnHidden()
    {
        // Arrange
        const int value = 0;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNegativeInteger_ShouldReturnVisible()
    {
        // Arrange
        const int value = -1;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnHidden()
    {
        // Arrange, Act & Assert
        var result = sut.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNotInteger_ShouldReturnHidden()
    {
        // Arrange
        const string value = "test";

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsDouble_ShouldReturnHidden()
    {
        // Arrange
        const double value = 3.5;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsLargeInteger_ShouldReturnVisible()
    {
        // Arrange
        const int value = 1000;

        // Act
        var result = sut.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange, Act & Assert
        var action = () => sut.ConvertBack(Visibility.Visible, typeof(int), null, CultureInfo.InvariantCulture);
        action.Should().Throw<NotImplementedException>();
    }
}