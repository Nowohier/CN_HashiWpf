using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;

namespace Hashi.Gui.Test.Converters;

/// <summary>
/// Unit tests for MaxMessageTextWidthConverter class.
/// </summary>
public class MaxMessageTextWidthConverterTests
{
    private MaxMessageTextWidthConverter sut;

    [SetUp]
    public void SetUp()
    {
        sut = new MaxMessageTextWidthConverter();
    }

    [Test]
    public void Convert_WhenValueIsValidDouble_ShouldReturnValueMinus30()
    {
        // Arrange
        const double value = 200.0;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(170.0);
    }

    [Test]
    public void Convert_WhenValueIsZero_ShouldReturnNegative30()
    {
        // Arrange
        const double value = 0.0;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(-30.0);
    }

    [Test]
    public void Convert_WhenValueIsNegative_ShouldReturnValueMinus30()
    {
        // Arrange
        const double value = -10.0;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(-40.0);
    }

    [Test]
    public void Convert_WhenValueIsLargeDouble_ShouldReturnValueMinus30()
    {
        // Arrange
        const double value = 1000.5;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(970.5);
    }

    [Test]
    public void Convert_WhenValueIsExactly30_ShouldReturnZero()
    {
        // Arrange
        const double value = 30.0;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0.0);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnZero()
    {
        // Arrange, Act & Assert
        var result = sut.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsNotDouble_ShouldReturnZero()
    {
        // Arrange
        const string value = "test";

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldReturnZero()
    {
        // Arrange
        const int value = 100;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsFloat_ShouldReturnZero()
    {
        // Arrange
        const float value = 100.5f;

        // Act
        var result = sut.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange, Act & Assert
        var action = () => sut.ConvertBack(170.0, typeof(double), null, CultureInfo.InvariantCulture);
        action.Should().Throw<NotImplementedException>();
    }
}