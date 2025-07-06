using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows.Data;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class MaxMessageTextWidthConverterTests
{
    private MaxMessageTextWidthConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new MaxMessageTextWidthConverter();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new MaxMessageTextWidthConverter();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsDouble_ShouldReturnValueMinus30()
    {
        // Arrange
        var value = 100.0;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(70.0);
    }

    [Test]
    public void Convert_WhenValueIsZero_ShouldReturnMinus30()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(-30.0);
    }

    [Test]
    public void Convert_WhenValueIsNegative_ShouldReturnValueMinus30()
    {
        // Arrange
        var value = -50.0;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(-80.0);
    }

    [Test]
    public void Convert_WhenValueIsLargeNumber_ShouldReturnValueMinus30()
    {
        // Arrange
        var value = 1000.0;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(970.0);
    }

    [Test]
    public void Convert_WhenValueIsNotDouble_ShouldReturn0()
    {
        // Arrange
        var value = "not a double";

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturn0()
    {
        // Act
        var result = converter.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldReturn0()
    {
        // Arrange
        var value = 100;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsFloat_ShouldReturn0()
    {
        // Arrange
        var value = 100f;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsDecimal_ShouldReturn0()
    {
        // Arrange
        var value = 100m;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenValueIsBoxedDouble_ShouldWorkCorrectly()
    {
        // Arrange
        object value = 150.0;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(120.0);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var act = () => converter.ConvertBack(70.0, typeof(double), null, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void Convert_WhenParameterAndCultureProvided_ShouldIgnoreThemAndWorkCorrectly()
    {
        // Arrange
        var value = 200.0;
        var parameter = "SomeParameter";
        var culture = new CultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, typeof(double), parameter, culture);

        // Assert
        result.Should().Be(170.0);
    }

    [Test]
    [TestCase(30.0, 0.0)]
    [TestCase(50.0, 20.0)]
    [TestCase(100.0, 70.0)]
    [TestCase(500.0, 470.0)]
    [TestCase(1000.0, 970.0)]
    public void Convert_WhenVariousDoubleValues_ShouldReturnCorrectResult(double inputValue, double expectedResult)
    {
        // Act
        var result = converter.Convert(inputValue, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase("string")]
    [TestCase(100)]
    [TestCase(100f)]
    [TestCase(100L)]
    [TestCase(true)]
    [TestCase(new object())]
    public void Convert_WhenValueIsNotDouble_ShouldAlwaysReturn0(object testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(0);
    }

    [Test]
    public void Convert_WhenTargetTypeIsNotDouble_ShouldStillWork()
    {
        // Arrange
        var value = 200.0;

        // Act
        var result = converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(170.0);
    }

    [Test]
    public void Convert_WhenMultipleCallsWithSameValue_ShouldReturnConsistentResults()
    {
        // Arrange
        var value = 250.0;

        // Act
        var result1 = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);
        var result2 = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result1.Should().Be(result2);
        result1.Should().Be(220.0);
    }

    [Test]
    public void Convert_WhenValueIsDoubleMaxValue_ShouldReturnMaxValueMinus30()
    {
        // Arrange
        var value = double.MaxValue;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(double.MaxValue - 30);
    }

    [Test]
    public void Convert_WhenValueIsDoubleMinValue_ShouldReturnMinValueMinus30()
    {
        // Arrange
        var value = double.MinValue;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(double.MinValue - 30);
    }

    [Test]
    public void Convert_WhenValueIsPositiveInfinity_ShouldReturnPositiveInfinity()
    {
        // Arrange
        var value = double.PositiveInfinity;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(double.PositiveInfinity);
    }

    [Test]
    public void Convert_WhenValueIsNegativeInfinity_ShouldReturnNegativeInfinity()
    {
        // Arrange
        var value = double.NegativeInfinity;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(double.NegativeInfinity);
    }

    [Test]
    public void Convert_WhenValueIsNaN_ShouldReturnNaN()
    {
        // Arrange
        var value = double.NaN;

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(double.NaN);
    }
}