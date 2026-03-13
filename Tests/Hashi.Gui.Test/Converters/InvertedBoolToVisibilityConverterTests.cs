using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class InvertedBoolToVisibilityConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new InvertedBoolToVisibilityConverter();
    }

    private InvertedBoolToVisibilityConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        var action = () => new InvertedBoolToVisibilityConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsTrue_ShouldReturnCollapsed()
    {
        // Arrange
        object value = true;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsFalse_ShouldReturnVisible()
    {
        // Arrange
        object value = false;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnVisible()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

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
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenCultureIsDifferent_ShouldStillWork()
    {
        // Arrange
        object value = false;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
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

    [Test]
    [TestCase(true, Visibility.Collapsed)]
    [TestCase(false, Visibility.Visible)]
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
