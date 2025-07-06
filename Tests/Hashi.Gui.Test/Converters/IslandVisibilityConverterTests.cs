using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class IslandVisibilityConverterTests
{
    private IslandVisibilityConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new IslandVisibilityConverter();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new IslandVisibilityConverter();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsZero_ShouldReturnHidden()
    {
        // Arrange
        var value = 0;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsPositiveInteger_ShouldReturnVisible()
    {
        // Arrange
        var value = 5;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNegativeInteger_ShouldReturnVisible()
    {
        // Arrange
        var value = -3;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnHidden()
    {
        // Act
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsNotInteger_ShouldReturnHidden()
    {
        // Arrange
        var value = "not an integer";

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsDouble_ShouldReturnHidden()
    {
        // Arrange
        var value = 5.5;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenValueIsBoxedInteger_ShouldWorkCorrectly()
    {
        // Arrange
        object value = 3;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsBoxedZero_ShouldReturnHidden()
    {
        // Arrange
        object value = 0;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var act = () => converter.ConvertBack(Visibility.Visible, typeof(int), null, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void Convert_WhenParameterAndCultureProvided_ShouldIgnoreThemAndWorkCorrectly()
    {
        // Arrange
        var value = 5;
        var parameter = "SomeParameter";
        var culture = new CultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, typeof(Visibility), parameter, culture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    [TestCase(0, Visibility.Hidden)]
    [TestCase(1, Visibility.Visible)]
    [TestCase(5, Visibility.Visible)]
    [TestCase(-1, Visibility.Visible)]
    [TestCase(-10, Visibility.Visible)]
    [TestCase(100, Visibility.Visible)]
    [TestCase(int.MaxValue, Visibility.Visible)]
    [TestCase(int.MinValue, Visibility.Visible)]
    public void Convert_WhenVariousIntegerValues_ShouldReturnCorrectVisibility(int inputValue, Visibility expectedResult)
    {
        // Act
        var result = converter.Convert(inputValue, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    [TestCase("string")]
    [TestCase(5.5)]
    [TestCase(true)]
    [TestCase(new object())]
    [TestCase((byte)5)]
    [TestCase((short)5)]
    [TestCase(5L)]
    [TestCase(5f)]
    public void Convert_WhenValueIsNotInt_ShouldAlwaysReturnHidden(object testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenTargetTypeIsNotVisibility_ShouldStillWork()
    {
        // Arrange
        var value = 5;

        // Act
        var result = converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenMultipleCallsWithSameValue_ShouldReturnConsistentResults()
    {
        // Arrange
        var value = 3;

        // Act
        var result1 = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        var result2 = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result1.Should().Be(result2);
        result1.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenNullableIntegerWithValue_ShouldReturnVisible()
    {
        // Arrange
        int? value = 5;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenNullableIntegerWithZero_ShouldReturnHidden()
    {
        // Arrange
        int? value = 0;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }

    [Test]
    public void Convert_WhenNullableIntegerIsNull_ShouldReturnHidden()
    {
        // Arrange
        int? value = null;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Hidden);
    }
}