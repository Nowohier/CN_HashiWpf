using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class BoolToVisibilityConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new BoolToVisibilityConverter();
    }

    private BoolToVisibilityConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var action = () => new BoolToVisibilityConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        // Act & Assert
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsTrue_ShouldReturnVisible()
    {
        // Arrange
        object value = true;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsFalse_ShouldReturnCollapsed()
    {
        // Arrange
        object value = false;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnCollapsed()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsBoolTrue_ShouldReturnVisible()
    {
        // Arrange
        var value = true;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsBoolFalse_ShouldReturnCollapsed()
    {
        // Arrange
        var value = false;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenTargetTypeIsNull_ShouldStillWork()
    {
        // Arrange
        object value = true;
        Type? targetType = null;
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenParameterIsNotNull_ShouldStillWork()
    {
        // Arrange
        object value = true;
        var targetType = typeof(Visibility);
        object parameter = "someParameter";
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenCultureIsNull_ShouldStillWork()
    {
        // Arrange
        object value = true;
        var targetType = typeof(Visibility);
        object? parameter = null;
        CultureInfo? culture = null;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenCultureIsDifferent_ShouldStillWork()
    {
        // Arrange
        object value = true;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNonBooleanType_ShouldThrowInvalidCastException()
    {
        // Arrange
        object value = "not a boolean";
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act & Assert
        var action = () => converter.Convert(value, targetType, parameter, culture);
        action.Should().Throw<InvalidCastException>();
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldThrowInvalidCastException()
    {
        // Arrange
        object value = 1;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act & Assert
        var action = () => converter.Convert(value, targetType, parameter, culture);
        action.Should().Throw<InvalidCastException>();
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange
        object value = Visibility.Visible;
        var targetType = typeof(bool);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act & Assert
        var action = () => converter.ConvertBack(value, targetType, parameter, culture);
        action.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void ConvertBack_WhenCalledWithDifferentParameters_ShouldThrowNotImplementedException()
    {
        // Arrange
        object value = Visibility.Collapsed;
        var targetType = typeof(bool);
        object parameter = "test";
        var culture = CultureInfo.GetCultureInfo("fr-FR");

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

    [Test]
    [TestCase(true, Visibility.Visible)]
    [TestCase(false, Visibility.Collapsed)]
    public void Convert_WhenBooleanValues_ShouldReturnCorrectVisibility(bool input, Visibility expected)
    {
        // Arrange
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(input, targetType, parameter, culture);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void Convert_ResultType_ShouldBeVisibility()
    {
        // Arrange
        object value = true;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().BeOfType<Visibility>();
    }
}