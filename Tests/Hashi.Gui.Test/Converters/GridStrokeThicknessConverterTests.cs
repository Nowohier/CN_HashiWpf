using System.Globalization;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class GridStrokeThicknessConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new GridStrokeThicknessConverter();
    }

    private GridStrokeThicknessConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        var action = () => new GridStrokeThicknessConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnOne()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(int);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void Convert_WhenValueIsNotItemsControl_ShouldReturnOne()
    {
        // Arrange
        object value = "not an ItemsControl";
        var targetType = typeof(int);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldReturnOne()
    {
        // Arrange
        object value = 42;
        var targetType = typeof(int);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void Convert_WhenParameterIsNotNull_ShouldStillWork()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(int);
        object parameter = "someParameter";
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void Convert_WhenCultureIsDifferent_ShouldStillWork()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(int);
        object? parameter = null;
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(1);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange
        object value = 1;
        var targetType = typeof(object);
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
