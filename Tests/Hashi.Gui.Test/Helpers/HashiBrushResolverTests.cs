using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Helpers;
using Hashi.Gui.Interfaces.Wrappers;
using Moq;
using System.Windows.Media;

namespace Hashi.Gui.Test.Helpers;

/// <summary>
/// Unit tests for <see cref="HashiBrushResolver"/> class.
/// </summary>
[TestFixture]
public class HashiBrushResolverTests
{
    private Mock<IApplicationWrapper> applicationWrapperMock;
    private HashiBrushResolver resolver;

    [SetUp]
    public void SetUp()
    {
        applicationWrapperMock = new Mock<IApplicationWrapper>(MockBehavior.Strict);
        resolver = new HashiBrushResolver(applicationWrapperMock.Object);
    }

    [Test]
    public void ResolveBrush_WhenBrushExists_ShouldReturnHashiBrush()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Red);
        applicationWrapperMock
            .Setup(a => a.GetApplicationResource(HashiColor.BasicBrush.ToString()))
            .Returns(brush);

        // Act
        var result = resolver.ResolveBrush(HashiColor.BasicBrush);

        // Assert
        result.Should().NotBeNull();
        result.Brush.Should().Be(brush);
    }

    [Test]
    public void ResolveBrush_WhenBrushNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        applicationWrapperMock
            .Setup(a => a.GetApplicationResource(HashiColor.BasicBrush.ToString()))
            .Returns(null);

        // Act & Assert
        var action = () => resolver.ResolveBrush(HashiColor.BasicBrush);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("color")
            .WithMessage("*No brush found for color*");
    }

    [Test]
    public void ResolveBrush_WhenResourceIsNotSolidColorBrush_ShouldThrowArgumentException()
    {
        // Arrange
        applicationWrapperMock
            .Setup(a => a.GetApplicationResource(HashiColor.BasicBrush.ToString()))
            .Returns("not a brush");

        // Act & Assert
        var action = () => resolver.ResolveBrush(HashiColor.BasicBrush);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("color");
    }
}
