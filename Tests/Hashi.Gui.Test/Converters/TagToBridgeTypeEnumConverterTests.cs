using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Converters;
using System.Globalization;

namespace Hashi.Gui.Test.Converters;

/// <summary>
/// Unit tests for TagToBridgeTypeEnumConverter class.
/// </summary>
public class TagToBridgeTypeEnumConverterTests
{
    private TagToBridgeTypeEnumConverter sut;

    [SetUp]
    public void SetUp()
    {
        sut = new TagToBridgeTypeEnumConverter();
    }

    [Test]
    public void Convert_WhenValueIsBridgeTypeEnum_ShouldReturnSameValue()
    {
        // Arrange
        const BridgeTypeEnum value = BridgeTypeEnum.Horizontal;

        // Act
        var result = sut.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(value);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnBridgeTypeEnumNone()
    {
        // Arrange, Act & Assert
        var result = sut.Convert(null, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsNotBridgeTypeEnum_ShouldReturnBridgeTypeEnumNone()
    {
        // Arrange
        const string value = "test";

        // Act
        var result = sut.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsVerticalBridgeType_ShouldReturnSameValue()
    {
        // Arrange
        const BridgeTypeEnum value = BridgeTypeEnum.VerticalDouble;

        // Act
        var result = sut.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(value);
    }

    [Test]
    public void Convert_WhenValueIsHorizontalDoubleBridgeType_ShouldReturnSameValue()
    {
        // Arrange
        const BridgeTypeEnum value = BridgeTypeEnum.HorizontalDouble;

        // Act
        var result = sut.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(value);
    }

    [Test]
    public void Convert_WhenValueIsNonEnumObject_ShouldReturnBridgeTypeEnumNone()
    {
        // Arrange
        const int value = 42;

        // Act
        var result = sut.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Arrange, Act & Assert
        var action = () => sut.ConvertBack(BridgeTypeEnum.Horizontal, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);
        action.Should().Throw<NotImplementedException>();
    }
}