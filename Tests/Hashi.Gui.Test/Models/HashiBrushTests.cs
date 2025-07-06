using FluentAssertions;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Models;
using System.Windows.Media;

namespace Hashi.Gui.Test.Models;

[TestFixture]
public class HashiBrushTests
{
    private HashiBrush hashiBrush;
    private SolidColorBrush testBrush;

    [SetUp]
    public void SetUp()
    {
        testBrush = new SolidColorBrush(Colors.Red);
        hashiBrush = new HashiBrush(testBrush);
    }

    [Test]
    public void Constructor_WhenValidBrush_ShouldInitializeBrushProperty()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Blue);

        // Act
        var result = new HashiBrush(brush);

        // Assert
        result.Brush.Should().Be(brush);
    }

    [Test]
    public void Constructor_WhenNullBrush_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HashiBrush(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Brush_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Act
        var result = hashiBrush.Brush;

        // Assert
        result.Should().Be(testBrush);
    }

    [Test]
    public void HashiBrush_ShouldImplementIHashiBrush()
    {
        // Act & Assert
        hashiBrush.Should().BeAssignableTo<IHashiBrush>();
    }

    [Test]
    public void Brush_WhenAccessed_ShouldReturnObjectType()
    {
        // Act
        var result = hashiBrush.Brush;

        // Assert
        result.Should().BeAssignableTo<object>();
        result.Should().BeAssignableTo<SolidColorBrush>();
    }

    [Test]
    public void Brush_Property_ShouldBeReadOnly()
    {
        // Act & Assert
        var brushProperty = typeof(HashiBrush).GetProperty(nameof(HashiBrush.Brush));
        brushProperty.Should().NotBeNull();
        brushProperty!.CanWrite.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Arrange
        var brush1 = new SolidColorBrush(Colors.Green);
        var brush2 = new SolidColorBrush(Colors.Yellow);

        // Act
        var hashiBrush1 = new HashiBrush(brush1);
        var hashiBrush2 = new HashiBrush(brush2);

        // Assert
        hashiBrush1.Should().NotBeSameAs(hashiBrush2);
        hashiBrush1.Brush.Should().NotBe(hashiBrush2.Brush);
        hashiBrush1.Brush.Should().Be(brush1);
        hashiBrush2.Brush.Should().Be(brush2);
    }

    [Test]
    [TestCase(255, 0, 0, 255)] // Red
    [TestCase(0, 255, 0, 255)] // Green
    [TestCase(0, 0, 255, 255)] // Blue
    [TestCase(255, 255, 255, 255)] // White
    [TestCase(0, 0, 0, 255)] // Black
    public void Constructor_WhenDifferentColors_ShouldAcceptAllColors(byte a, byte r, byte g, byte b)
    {
        // Arrange
        var color = Color.FromArgb(a, r, g, b);
        var brush = new SolidColorBrush(color);

        // Act
        var result = new HashiBrush(brush);

        // Assert
        result.Brush.Should().Be(brush);
        ((SolidColorBrush)result.Brush).Color.Should().Be(color);
    }

    [Test]
    public void Constructor_WhenBrushWithTransparency_ShouldAcceptTransparentBrush()
    {
        // Arrange
        var transparentColor = Color.FromArgb(128, 255, 0, 0); // Semi-transparent red
        var transparentBrush = new SolidColorBrush(transparentColor);

        // Act
        var result = new HashiBrush(transparentBrush);

        // Assert
        result.Brush.Should().Be(transparentBrush);
        ((SolidColorBrush)result.Brush).Color.A.Should().Be(128);
    }

    [Test]
    public void Constructor_WhenBrushIsFrozen_ShouldAcceptFrozenBrush()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Purple);
        brush.Freeze();

        // Act
        var result = new HashiBrush(brush);

        // Assert
        result.Brush.Should().Be(brush);
        ((SolidColorBrush)result.Brush).IsFrozen.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenSameBrushInstanceUsedMultipleTimes_ShouldReferenceSameBrush()
    {
        // Arrange
        var sharedBrush = new SolidColorBrush(Colors.Orange);

        // Act
        var hashiBrush1 = new HashiBrush(sharedBrush);
        var hashiBrush2 = new HashiBrush(sharedBrush);

        // Assert
        hashiBrush1.Brush.Should().BeSameAs(hashiBrush2.Brush);
        hashiBrush1.Brush.Should().BeSameAs(sharedBrush);
        hashiBrush2.Brush.Should().BeSameAs(sharedBrush);
    }

    [Test]
    public void Brush_WhenCastToSolidColorBrush_ShouldMaintainProperties()
    {
        // Arrange
        var originalColor = Colors.Cyan;
        var originalBrush = new SolidColorBrush(originalColor);

        // Act
        var result = new HashiBrush(originalBrush);
        var castBrush = (SolidColorBrush)result.Brush;

        // Assert
        castBrush.Color.Should().Be(originalColor);
        castBrush.Should().BeSameAs(originalBrush);
    }
}