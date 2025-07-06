using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.Models;
using System.Drawing;

namespace Hashi.Gui.Test.Extensions;

[TestFixture]
public class HashiPointExtensionsTests
{
    [Test]
    public void ToHashiPoint_WhenCalledWithPoint_ShouldReturnHashiPointWithSameCoordinates()
    {
        // Arrange
        var point = new Point(10, 20);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(10);
        result.Y.Should().Be(20);
        result.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void ToHashiPoint_WhenCalledWithPointAndPointType_ShouldReturnHashiPointWithSpecifiedType()
    {
        // Arrange
        var point = new Point(5, 15);
        var pointType = HashiPointTypeEnum.Island;

        // Act
        var result = point.ToHashiPoint(pointType);

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(5);
        result.Y.Should().Be(15);
        result.PointType.Should().Be(pointType);
    }

    [Test]
    public void ToHashiPoint_WhenCalledWithZeroCoordinates_ShouldReturnHashiPointWithZeroCoordinates()
    {
        // Arrange
        var point = new Point(0, 0);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(0);
        result.Y.Should().Be(0);
        result.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void ToHashiPoint_WhenCalledWithNegativeCoordinates_ShouldReturnHashiPointWithNegativeCoordinates()
    {
        // Arrange
        var point = new Point(-10, -20);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(-10);
        result.Y.Should().Be(-20);
        result.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void ToHashiPoint_WhenCalledWithLargeCoordinates_ShouldReturnHashiPointWithLargeCoordinates()
    {
        // Arrange
        var point = new Point(int.MaxValue, int.MaxValue);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(int.MaxValue);
        result.Y.Should().Be(int.MaxValue);
        result.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void ToHashiPoint_WhenCalledWithMinimumCoordinates_ShouldReturnHashiPointWithMinimumCoordinates()
    {
        // Arrange
        var point = new Point(int.MinValue, int.MinValue);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(int.MinValue);
        result.Y.Should().Be(int.MinValue);
        result.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    [TestCase(HashiPointTypeEnum.Normal)]
    [TestCase(HashiPointTypeEnum.Island)]
    [TestCase(HashiPointTypeEnum.Connection)]
    public void ToHashiPoint_WhenCalledWithDifferentPointTypes_ShouldReturnCorrectPointType(HashiPointTypeEnum pointType)
    {
        // Arrange
        var point = new Point(1, 2);

        // Act
        var result = point.ToHashiPoint(pointType);

        // Assert
        result.Should().NotBeNull();
        result.PointType.Should().Be(pointType);
        result.X.Should().Be(1);
        result.Y.Should().Be(2);
    }

    [Test]
    public void ToHashiPoint_ShouldReturnIHashiPoint()
    {
        // Arrange
        var point = new Point(3, 4);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.Should().BeAssignableTo<IHashiPoint>();
    }

    [Test]
    public void ToHashiPoint_WhenCalledMultipleTimes_ShouldReturnIndependentInstances()
    {
        // Arrange
        var point = new Point(7, 8);

        // Act
        var result1 = point.ToHashiPoint();
        var result2 = point.ToHashiPoint();

        // Assert
        result1.Should().NotBeSameAs(result2);
        result1.X.Should().Be(result2.X);
        result1.Y.Should().Be(result2.Y);
        result1.PointType.Should().Be(result2.PointType);
    }

    [Test]
    public void ToHashiPoint_WhenCalledWithSamePointButDifferentTypes_ShouldReturnDifferentPointTypes()
    {
        // Arrange
        var point = new Point(9, 10);

        // Act
        var normalPoint = point.ToHashiPoint(HashiPointTypeEnum.Normal);
        var islandPoint = point.ToHashiPoint(HashiPointTypeEnum.Island);

        // Assert
        normalPoint.X.Should().Be(islandPoint.X);
        normalPoint.Y.Should().Be(islandPoint.Y);
        normalPoint.PointType.Should().NotBe(islandPoint.PointType);
        normalPoint.PointType.Should().Be(HashiPointTypeEnum.Normal);
        islandPoint.PointType.Should().Be(HashiPointTypeEnum.Island);
    }

    [Test]
    public void ToHashiPoint_WithDefaultParameter_ShouldUseNormalPointType()
    {
        // Arrange
        var point = new Point(11, 12);

        // Act
        var result = point.ToHashiPoint();

        // Assert
        result.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void ToHashiPoint_WhenCalledOnDifferentPoints_ShouldReturnDifferentCoordinates()
    {
        // Arrange
        var point1 = new Point(1, 2);
        var point2 = new Point(3, 4);

        // Act
        var result1 = point1.ToHashiPoint();
        var result2 = point2.ToHashiPoint();

        // Assert
        result1.X.Should().NotBe(result2.X);
        result1.Y.Should().NotBe(result2.Y);
        result1.X.Should().Be(1);
        result1.Y.Should().Be(2);
        result2.X.Should().Be(3);
        result2.Y.Should().Be(4);
    }

    [Test]
    public void ToHashiPoint_ExtensionMethod_ShouldBeAccessibleOnPointType()
    {
        // Arrange
        var point = new Point(13, 14);

        // Act & Assert
        // If this compiles and runs, the extension method is properly accessible
        var result = point.ToHashiPoint();
        result.Should().NotBeNull();
    }

    [Test]
    public void ToHashiPoint_WhenInvalidPointType_ShouldStillAcceptValue()
    {
        // Arrange
        var point = new Point(15, 16);
        var invalidPointType = (HashiPointTypeEnum)999;

        // Act
        var result = point.ToHashiPoint(invalidPointType);

        // Assert
        result.Should().NotBeNull();
        result.X.Should().Be(15);
        result.Y.Should().Be(16);
        result.PointType.Should().Be(invalidPointType);
    }

    [Test]
    public void ToHashiPoint_MethodSignature_ShouldHaveCorrectParameters()
    {
        // Arrange
        var method = typeof(HashiPointExtensions).GetMethod(nameof(HashiPointExtensions.ToHashiPoint));

        // Assert
        method.Should().NotBeNull();
        method!.IsStatic.Should().BeTrue();
        method.IsPublic.Should().BeTrue();
        method.ReturnType.Should().Be(typeof(IHashiPoint));
        
        var parameters = method.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(Point));
        parameters[1].ParameterType.Should().Be(typeof(HashiPointTypeEnum));
        parameters[1].HasDefaultValue.Should().BeTrue();
        parameters[1].DefaultValue.Should().Be(HashiPointTypeEnum.Normal);
    }
}