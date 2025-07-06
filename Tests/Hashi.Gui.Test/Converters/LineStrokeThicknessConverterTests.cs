using FluentAssertions;
using Hashi.Gui.Converters;
using Moq;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hashi.Gui.Test.Converters;

[TestFixture]
public class LineStrokeThicknessConverterTests
{
    private LineStrokeThicknessConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new LineStrokeThicknessConverter();
    }

    [Test]
    public void Constructor_WhenCalled_ShouldCreateInstance()
    {
        // Act
        var result = new LineStrokeThicknessConverter();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IValueConverter>();
    }

    [Test]
    public void Convert_WhenValueIsNotItemsControl_ShouldReturn1Point5()
    {
        // Arrange
        var value = "not an ItemsControl";

        // Act
        var result = converter.Convert(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenValueIsNull_ShouldReturn1Point5()
    {
        // Act
        var result = converter.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlWidthIsGreaterThan1000_ShouldReturn2()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1001);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(500);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void Convert_WhenItemsControlHeightIsGreaterThan1000_ShouldReturn2()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(500);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(1001);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void Convert_WhenItemsControlBothDimensionsGreaterThan1000_ShouldReturn2()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1200);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(1500);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void Convert_WhenItemsControlBothDimensionsLessOrEqualTo1000_ShouldReturn1Point5()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(800);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(600);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlWidthExactly1000_ShouldReturn1Point5()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1000);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(500);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlHeightExactly1000_ShouldReturn1Point5()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(500);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(1000);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlBothDimensionsExactly1000_ShouldReturn1Point5()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1000);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(1000);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlWithZeroDimensions_ShouldReturn1Point5()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(0);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(0);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlWithNegativeDimensions_ShouldReturn1Point5()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(-100);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(-50);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void ConvertBack_WhenCalled_ShouldThrowNotImplementedException()
    {
        // Act & Assert
        var act = () => converter.ConvertBack(2.0, typeof(ItemsControl), null, CultureInfo.InvariantCulture);
        act.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void Convert_WhenParameterAndCultureProvided_ShouldIgnoreThemAndWorkCorrectly()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1200);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(800);
        var parameter = "SomeParameter";
        var culture = new CultureInfo("de-DE");

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), parameter, culture);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    [TestCase(500, 500, 1.5)]
    [TestCase(1000, 500, 1.5)]
    [TestCase(500, 1000, 1.5)]
    [TestCase(1000, 1000, 1.5)]
    [TestCase(1001, 500, 2)]
    [TestCase(500, 1001, 2)]
    [TestCase(1001, 1001, 2)]
    [TestCase(2000, 800, 2)]
    [TestCase(800, 2000, 2)]
    public void Convert_WhenVariousDimensions_ShouldReturnCorrectThickness(double width, double height, double expectedResult)
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(width);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(height);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Test]
    public void Convert_WhenTargetTypeIsNotDouble_ShouldStillWork()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1200);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(800);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(object), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(2);
    }

    [Test]
    public void Convert_WhenMultipleCallsWithSameItemsControl_ShouldReturnConsistentResults()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(1200);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(800);

        // Act
        var result1 = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);
        var result2 = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result1.Should().Be(result2);
        result1.Should().Be(2);
    }

    [Test]
    [TestCase("string")]
    [TestCase(123)]
    [TestCase(true)]
    [TestCase(12.34)]
    public void Convert_WhenValueIsNotItemsControl_ShouldAlwaysReturn1Point5(object testValue)
    {
        // Act
        var result = converter.Convert(testValue, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(1.5);
    }

    [Test]
    public void Convert_WhenItemsControlWithVeryLargeDimensions_ShouldReturn2()
    {
        // Arrange
        var itemsControlMock = new Mock<ItemsControl>();
        itemsControlMock.SetupGet(x => x.ActualWidth).Returns(double.MaxValue);
        itemsControlMock.SetupGet(x => x.ActualHeight).Returns(double.MaxValue);

        // Act
        var result = converter.Convert(itemsControlMock.Object, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        result.Should().Be(2);
    }
}