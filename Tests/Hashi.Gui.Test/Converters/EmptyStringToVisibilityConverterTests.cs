using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class EmptyStringToVisibilityConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new EmptyStringToVisibilityConverter();
    }

    private EmptyStringToVisibilityConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        var action = () => new EmptyStringToVisibilityConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsNonEmptyString_ShouldReturnVisible()
    {
        // Arrange
        object value = "hello";
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsEmptyString_ShouldReturnHidden()
    {
        // Arrange
        object value = "";
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnHidden()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNotString_ShouldReturnHidden()
    {
        // Arrange
        object value = 42;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsWhitespace_ShouldReturnVisible()
    {
        // Arrange
        object value = "   ";
        var targetType = typeof(Visibility);
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
        object value = "test";
        var targetType = typeof(Visibility);
        object parameter = "someParameter";
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenCultureIsDifferent_ShouldStillWork()
    {
        // Arrange
        object value = "test";
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
        object value = "test";
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().BeOfType<Visibility>();
    }

    [Test]
    [TestCase("hello", Visibility.Visible)]
    [TestCase("", Visibility.Hidden)]
    public void Convert_WhenStringValues_ShouldReturnCorrectVisibility(string input, Visibility expected)
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
        var targetType = typeof(string);
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
