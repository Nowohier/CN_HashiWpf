using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows.Data;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class TagToBridgeTypeEnumConverterTests
{
    private TagToBridgeTypeEnumConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new TagToBridgeTypeEnumConverter();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new TagToBridgeTypeEnumConverter();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsBridgeTypeEnum_ShouldReturnSameValue()
    {
        // Arrange
        var value = BridgeTypeEnum.Single;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Single);
    }

    [Test]
    public void Convert_WhenValueIsBridgeTypeEnumDouble_ShouldReturnSameValue()
    {
        // Arrange
        var value = BridgeTypeEnum.Double;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Double);
    }

    [Test]
    public void Convert_WhenValueIsBridgeTypeEnumNone_ShouldReturnSameValue()
    {
        // Arrange
        var value = BridgeTypeEnum.None;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsNotBridgeTypeEnum_ShouldReturnNone()
    {
        // Arrange
        var value = "not a BridgeTypeEnum";

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnNone()
    {
        // Act
        var result = converter.Convert(null, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldReturnNone()
    {
        // Arrange
        var value = 1;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenValueIsBoxedBridgeTypeEnum_ShouldWorkCorrectly()
    {
        // Arrange
        object value = BridgeTypeEnum.Double;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Double);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var act = () => converter.ConvertBack(BridgeTypeEnum.Single, typeof(object), null, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void Convert_WhenParameterAndCultureProvided_ShouldIgnoreThemAndWorkCorrectly()
    {
        // Arrange
        var value = BridgeTypeEnum.Single;
        var parameter = "SomeParameter";
        var culture = new CultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), parameter, culture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Single);
    }

    [Test]
    [TestCase(BridgeTypeEnum.None, BridgeTypeEnum.None)]
    [TestCase(BridgeTypeEnum.Single, BridgeTypeEnum.Single)]
    [TestCase(BridgeTypeEnum.Double, BridgeTypeEnum.Double)]
    public void Convert_WhenValidBridgeTypeEnumValues_ShouldReturnSameValue(BridgeTypeEnum inputValue, BridgeTypeEnum expectedResult)
    {
        // Act
        var result = converter.Convert(inputValue, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase("string")]
    [TestCase(123)]
    [TestCase(true)]
    [TestCase(45.67)]
    [TestCase(new object())]
    public void Convert_WhenValueIsNotBridgeTypeEnum_ShouldAlwaysReturnNone(object testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenTargetTypeIsNotBridgeTypeEnum_ShouldStillWork()
    {
        // Arrange
        var value = BridgeTypeEnum.Double;

        // Act
        var result = converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Double);
    }

    [Test]
    public void Convert_WhenMultipleCallsWithSameValue_ShouldReturnConsistentResults()
    {
        // Arrange
        var value = BridgeTypeEnum.Single;

        // Act
        var result1 = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);
        var result2 = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result1.Should().Be(result2);
        result1.Should().Be(BridgeTypeEnum.Single);
    }

    [Test]
    public void Convert_WhenInvalidBridgeTypeEnumValue_ShouldReturnSameValue()
    {
        // Arrange
        var invalidValue = (BridgeTypeEnum)999;

        // Act
        var result = converter.Convert(invalidValue, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(invalidValue);
    }

    [Test]
    public void Convert_WhenBooleanValue_ShouldReturnNone()
    {
        // Arrange
        var value = true;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenDoubleValue_ShouldReturnNone()
    {
        // Arrange
        var value = 3.14;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenArrayValue_ShouldReturnNone()
    {
        // Arrange
        var value = new[] { 1, 2, 3 };

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenOtherEnumValue_ShouldReturnNone()
    {
        // Arrange
        var value = DifficultyEnum.Easy; // Different enum type

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }

    [Test]
    public void Convert_WhenNullableBridgeTypeEnum_ShouldWorkCorrectly()
    {
        // Arrange
        BridgeTypeEnum? value = BridgeTypeEnum.Single;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.Single);
    }

    [Test]
    public void Convert_WhenNullableBridgeTypeEnumIsNull_ShouldReturnNone()
    {
        // Arrange
        BridgeTypeEnum? value = null;

        // Act
        var result = converter.Convert(value, typeof(BridgeTypeEnum), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(BridgeTypeEnum.None);
    }
}