using FluentAssertions;
using Hashi.Gui.Converters;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class InvertedBoolToVisibilityConverterTests
{
    private InvertedBoolToVisibilityConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new InvertedBoolToVisibilityConverter();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new InvertedBoolToVisibilityConverter();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsTrue_ShouldReturnCollapsed()
    {
        // Arrange
        var value = true;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsFalse_ShouldReturnVisible()
    {
        // Arrange
        var value = false;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturnVisible()
    {
        // Act
        var result = converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNotBool_ShouldThrowInvalidCastException()
    {
        // Arrange
        var value = "not a boolean";

        // Act & Assert
        var act = () => converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        act.Should().Throw<InvalidCastException>();
    }

    [Test]
    public void Convert_WhenValueIsInteger_ShouldThrowInvalidCastException()
    {
        // Arrange
        var value = 123;

        // Act & Assert
        var act = () => converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        act.Should().Throw<InvalidCastException>();
    }

    [Test]
    public void Convert_WhenValueIsBoxedTrue_ShouldReturnCollapsed()
    {
        // Arrange
        object value = true;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsBoxedFalse_ShouldReturnVisible()
    {
        // Arrange
        object value = false;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var act = () => converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void Convert_WhenParameterAndCultureProvided_ShouldIgnoreThemAndWorkCorrectly()
    {
        // Arrange
        var value = true;
        var parameter = "SomeParameter";
        var culture = new CultureInfo("de-DE");

        // Act
        var result = converter.Convert(value, typeof(Visibility), parameter, culture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    [TestCase(true, Visibility.Collapsed)]
    [TestCase(false, Visibility.Visible)]
    public void Convert_WhenBooleanValues_ShouldReturnInvertedVisibility(bool inputValue, Visibility expectedResult)
    {
        // Act
        var result = converter.Convert(inputValue, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    public void Convert_WhenTargetTypeIsNotVisibility_ShouldStillWork()
    {
        // Arrange
        var value = true;

        // Act
        var result = converter.Convert(value, typeof(object), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsNullableTrue_ShouldReturnCollapsed()
    {
        // Arrange
        bool? value = true;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Test]
    public void Convert_WhenValueIsNullableFalse_ShouldReturnVisible()
    {
        // Arrange
        bool? value = false;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenValueIsNullableNull_ShouldReturnVisible()
    {
        // Arrange
        bool? value = null;

        // Act
        var result = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Test]
    public void Convert_WhenMultipleCallsWithSameValue_ShouldReturnConsistentResults()
    {
        // Arrange
        var value = true;

        // Act
        var result1 = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);
        var result2 = converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture);

        // Assert
        result1.Should().Be(result2);
        result1.Should().Be(Visibility.Collapsed);
    }
}