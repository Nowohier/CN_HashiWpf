using System.Globalization;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class MaxMessageTextWidthConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new MaxMessageTextWidthConverter();
    }

    private MaxMessageTextWidthConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        var action = () => new MaxMessageTextWidthConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsDouble_ShouldReturnValueMinus30()
    {
        // Arrange
        object value = 500.0;
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(470.0);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnZero()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsNotDouble_ShouldReturnZero()
    {
        // Arrange
        object value = "not a double";
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldReturnZero()
    {
        // Arrange
        object value = 500;
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    [TestCase(100.0, 70.0)]
    [TestCase(30.0, 0.0)]
    [TestCase(0.0, -30.0)]
    [TestCase(1000.0, 970.0)]
    public void Convert_WhenDoubleValues_ShouldSubtract30(double input, double expected)
    {
        // Arrange
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(input, targetType, parameter, culture);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void Convert_WhenParameterIsNotNull_ShouldStillWork()
    {
        // Arrange
        object value = 200.0;
        var targetType = typeof(double);
        object parameter = "someParameter";
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(170.0);
    }

    [Test]
    public void Convert_WhenCultureIsDifferent_ShouldStillWork()
    {
        // Arrange
        object value = 200.0;
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(170.0);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange
        object value = 100.0;
        var targetType = typeof(double);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act & Assert
        var action = () => converter.ConvertBack(value, targetType, parameter, culture);
        action.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void ConvertBack_WhenCalledWithNullValues_ShouldThrowNotImplementedException()
    {
        // Arrange
        object? value = null;
        Type? targetType = null;
        object? parameter = null;
        CultureInfo? culture = null;

        // Act & Assert
        var action = () => converter.ConvertBack(value, targetType, parameter, culture);
        action.Should().Throw<NotImplementedException>();
    }
}
