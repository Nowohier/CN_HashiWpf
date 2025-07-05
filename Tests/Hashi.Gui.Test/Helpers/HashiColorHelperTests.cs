using System.Windows;
using System.Windows.Media;
using FluentAssertions;
using Hashi.Gui.Helpers;

namespace Hashi.Gui.Test.Helpers;

[TestFixture]
public class HashiColorHelperTests
{
    [SetUp]
    public void SetUp()
    {
        // Create a mock application to provide resources
        if (Application.Current == null)
        {
            var app = new Application();
            app.Resources = new ResourceDictionary();

            // Add mock brushes
            app.Resources[nameof(HashiColorHelper.MenuBackgroundBrush)] = Brushes.Gray;
            app.Resources[nameof(HashiColorHelper.BackgroundBrush)] = Brushes.White;
            app.Resources[nameof(HashiColorHelper.BasicBrush)] = Brushes.Blue;
            app.Resources[nameof(HashiColorHelper.BasicIslandBrush)] = Brushes.LightBlue;
            app.Resources[nameof(HashiColorHelper.GreenIslandBrush)] = Brushes.Green;
            app.Resources[nameof(HashiColorHelper.IntenseGreenBrush)] = Brushes.DarkGreen;
            app.Resources[nameof(HashiColorHelper.MaxBridgesReachedBrush)] = Brushes.Red;
            app.Resources[nameof(HashiColorHelper.PotentialConnectionBrush)] = Brushes.Yellow;
            app.Resources[nameof(HashiColorHelper.GridLineBrush)] = Brushes.LightGray;
            app.Resources[nameof(HashiColorHelper.TestModeBrush)] = Brushes.Orange;
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the application if needed
        Application.Current?.Shutdown();
    }

    [Test]
    public void MenuBackgroundBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.MenuBackgroundBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.Gray);
    }

    [Test]
    public void BackgroundBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.BackgroundBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.White);
    }

    [Test]
    public void BasicBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.BasicBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.Blue);
    }

    [Test]
    public void BasicIslandBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.BasicIslandBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.LightBlue);
    }

    [Test]
    public void GreenIslandBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.GreenIslandBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.Green);
    }

    [Test]
    public void IntenseGreenBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.IntenseGreenBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.DarkGreen);
    }

    [Test]
    public void MaxBridgesReachedBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.MaxBridgesReachedBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.Red);
    }

    [Test]
    public void PotentialConnectionBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.PotentialConnectionBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.Yellow);
    }

    [Test]
    public void GridLineBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.GridLineBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.LightGray);
    }

    [Test]
    public void TestModeBrush_WhenAccessed_ShouldReturnResourceBrush()
    {
        // Act
        var brush = HashiColorHelper.TestModeBrush;

        // Assert
        brush.Should().NotBeNull();
        brush.Should().BeOfType<SolidColorBrush>();
        brush.Should().Be(Brushes.Orange);
    }

    [Test]
    public void AllBrushes_WhenAccessed_ShouldReturnSolidColorBrush()
    {
        // Act & Assert
        HashiColorHelper.MenuBackgroundBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.BackgroundBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.BasicBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.BasicIslandBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.GreenIslandBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.IntenseGreenBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.MaxBridgesReachedBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.PotentialConnectionBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.GridLineBrush.Should().BeOfType<SolidColorBrush>();
        HashiColorHelper.TestModeBrush.Should().BeOfType<SolidColorBrush>();
    }

    [Test]
    public void AllBrushes_WhenAccessed_ShouldNotBeNull()
    {
        // Act & Assert
        HashiColorHelper.MenuBackgroundBrush.Should().NotBeNull();
        HashiColorHelper.BackgroundBrush.Should().NotBeNull();
        HashiColorHelper.BasicBrush.Should().NotBeNull();
        HashiColorHelper.BasicIslandBrush.Should().NotBeNull();
        HashiColorHelper.GreenIslandBrush.Should().NotBeNull();
        HashiColorHelper.IntenseGreenBrush.Should().NotBeNull();
        HashiColorHelper.MaxBridgesReachedBrush.Should().NotBeNull();
        HashiColorHelper.PotentialConnectionBrush.Should().NotBeNull();
        HashiColorHelper.GridLineBrush.Should().NotBeNull();
        HashiColorHelper.TestModeBrush.Should().NotBeNull();
    }

    [Test]
    public void BrushProperties_WhenAccessedMultipleTimes_ShouldReturnSameInstance()
    {
        // Act
        var brush1 = HashiColorHelper.BasicBrush;
        var brush2 = HashiColorHelper.BasicBrush;

        // Assert
        brush1.Should().BeSameAs(brush2);
    }

    [Test]
    public void BrushProperties_ShouldBeStaticFields()
    {
        // Arrange
        var type = typeof(HashiColorHelper);

        // Act & Assert
        var menuBackgroundField = type.GetField(nameof(HashiColorHelper.MenuBackgroundBrush));
        var backgroundField = type.GetField(nameof(HashiColorHelper.BackgroundBrush));
        var basicField = type.GetField(nameof(HashiColorHelper.BasicBrush));
        var basicIslandField = type.GetField(nameof(HashiColorHelper.BasicIslandBrush));
        var greenIslandField = type.GetField(nameof(HashiColorHelper.GreenIslandBrush));
        var intenseGreenField = type.GetField(nameof(HashiColorHelper.IntenseGreenBrush));
        var maxBridgesField = type.GetField(nameof(HashiColorHelper.MaxBridgesReachedBrush));
        var potentialConnectionField = type.GetField(nameof(HashiColorHelper.PotentialConnectionBrush));
        var gridLineField = type.GetField(nameof(HashiColorHelper.GridLineBrush));
        var testModeField = type.GetField(nameof(HashiColorHelper.TestModeBrush));

        menuBackgroundField.Should().NotBeNull();
        backgroundField.Should().NotBeNull();
        basicField.Should().NotBeNull();
        basicIslandField.Should().NotBeNull();
        greenIslandField.Should().NotBeNull();
        intenseGreenField.Should().NotBeNull();
        maxBridgesField.Should().NotBeNull();
        potentialConnectionField.Should().NotBeNull();
        gridLineField.Should().NotBeNull();
        testModeField.Should().NotBeNull();

        menuBackgroundField.IsStatic.Should().BeTrue();
        backgroundField.IsStatic.Should().BeTrue();
        basicField.IsStatic.Should().BeTrue();
        basicIslandField.IsStatic.Should().BeTrue();
        greenIslandField.IsStatic.Should().BeTrue();
        intenseGreenField.IsStatic.Should().BeTrue();
        maxBridgesField.IsStatic.Should().BeTrue();
        potentialConnectionField.IsStatic.Should().BeTrue();
        gridLineField.IsStatic.Should().BeTrue();
        testModeField.IsStatic.Should().BeTrue();
    }
}