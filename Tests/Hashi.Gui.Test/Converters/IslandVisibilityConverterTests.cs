using System.Globalization;
using System.Windows;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class IslandVisibilityConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new IslandVisibilityConverter();
    }

    private IslandVisibilityConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        var action = () => new IslandVisibilityConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsPositiveInt_ShouldReturnVisible()
    {
        // Arrange
        object value = 3;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsZero_ShouldReturnHidden()
    {
        // Arrange
        object value = 0;
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
    public void Convert_WhenValueIsNotInt_ShouldReturnHidden()
    {
        // Arrange
        object value = "not an int";
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsDouble_ShouldReturnHidden()
    {
        // Arrange
        object value = 3.0;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNegativeInt_ShouldReturnVisible()
    {
        // Arrange
        object value = -1;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    [TestCase(1, Visibility.Visible)]
    [TestCase(5, Visibility.Visible)]
    [TestCase(8, Visibility.Visible)]
    [TestCase(0, Visibility.Hidden)]
    public void Convert_WhenIntValues_ShouldReturnCorrectVisibility(int input, Visibility expected)
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
    public void Convert_WhenParameterIsNotNull_ShouldStillWork()
    {
        // Arrange
        object value = 3;
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
        object value = 3;
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
        object value = 3;
        var targetType = typeof(Visibility);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().BeOfType<Visibility>();
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange
        object value = Visibility.Visible;
        var targetType = typeof(int);
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
