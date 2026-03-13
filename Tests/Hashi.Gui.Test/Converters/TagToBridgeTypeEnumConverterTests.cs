using System.Globalization;
using System.Windows.Data;
using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Converters;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class TagToBridgeTypeEnumConverterTests
{
    [SetUp]
    public void SetUp()
    {
        converter = new TagToBridgeTypeEnumConverter();
    }

    private TagToBridgeTypeEnumConverter converter;

    [Test]
    public void Constructor_WhenCreated_ShouldNotThrow()
    {
        var action = () => new TagToBridgeTypeEnumConverter();
        action.Should().NotThrow();
    }

    [Test]
    public void Converter_ShouldImplementIValueConverter()
    {
        converter.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsBridgeTypeEnum_ShouldReturnSameValue()
    {
        // Arrange
        object value = BridgeTypeEnum.Horizontal;
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Horizontal);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnNone()
    {
        // Arrange
        object? value = null;
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsNotBridgeTypeEnum_ShouldReturnNone()
    {
        // Arrange
        object value = "not a bridge type";
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldReturnNone()
    {
        // Arrange
        object value = 42;
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsBridgeTypeNone_ShouldReturnNone()
    {
        // Arrange
        object value = BridgeTypeEnum.None;
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenParameterIsNotNull_ShouldStillWork()
    {
        // Arrange
        object value = BridgeTypeEnum.Horizontal;
        var targetType = typeof(BridgeTypeEnum);
        object parameter = "someParameter";
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Horizontal);
    }

    [Test]
    public void Convert_WhenCultureIsDifferent_ShouldStillWork()
    {
        // Arrange
        object value = BridgeTypeEnum.Horizontal;
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.GetCultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Horizontal);
    }

    [Test]
    public void Convert_ResultType_ShouldBeBridgeTypeEnum()
    {
        // Arrange
        object value = BridgeTypeEnum.Horizontal;
        var targetType = typeof(BridgeTypeEnum);
        object? parameter = null;
        var culture = CultureInfo.InvariantCulture;

        // Act
        var result = converter.Convert(value, targetType, parameter, culture);

        // Assert
        result.Should().BeOfType<BridgeTypeEnum>();
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange
        object value = BridgeTypeEnum.Horizontal;
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
