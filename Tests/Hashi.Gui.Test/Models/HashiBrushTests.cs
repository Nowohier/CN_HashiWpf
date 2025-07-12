using FluentAssertions;
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

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalledWithValidBrush_ShouldInitializeBrush()
    {
        // Arrange
        var blueBrush = new SolidColorBrush(Colors.Blue);

        // Act
        var result = new HashiBrush(blueBrush);

        // Assert
        result.Brush.Should().Be(blueBrush);
    }

    [Test]
    public void Constructor_WhenCalledWithNullBrush_ShouldAllowNullValue()
    {
        // Act
        var result = new HashiBrush(null!);

        // Assert
        result.Brush.Should().BeNull();
    }

    #endregion

    #region Property Tests

    [Test]
    public void Brush_ShouldReturnCorrectValue()
    {
        // Act & Assert
        hashiBrush.Brush.Should().Be(testBrush);
    }

    [Test]
    public void Brush_WhenBrushIsValidSolidColorBrush_ShouldReturnAsObject()
    {
        // Act
        var result = hashiBrush.Brush;

        // Assert
        result.Should().BeOfType<SolidColorBrush>();
        result.Should().Be(testBrush);
    }

    #endregion

    #region Different Color Tests

    [Test]
    public void Constructor_WhenCalledWithDifferentColors_ShouldAcceptAllColors()
    {
        // Arrange & Act & Assert
        var colors = new[] { Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Purple, Colors.Orange };
        
        foreach (var color in colors)
        {
            var brush = new SolidColorBrush(color);
            var hashiBrush = new HashiBrush(brush);
            
            hashiBrush.Brush.Should().Be(brush);
            ((SolidColorBrush)hashiBrush.Brush).Color.Should().Be(color);
        }
    }

    [Test]
    public void Constructor_WhenCalledWithTransparentBrush_ShouldAcceptTransparency()
    {
        // Arrange
        var transparentBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0)); // Semi-transparent red

        // Act
        var result = new HashiBrush(transparentBrush);

        // Assert
        result.Brush.Should().Be(transparentBrush);
        ((SolidColorBrush)result.Brush).Color.A.Should().Be(128);
    }

    #endregion
}