using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Models;

namespace Hashi.Gui.Test.Models;

[TestFixture]
public class HashiPointTests
{
    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        var x = 5;
        var y = 10;
        var pointType = HashiPointTypeEnum.Hint;

        // Act
        var hashiPoint = new HashiPoint(x, y, pointType);

        // Assert
        hashiPoint.X.Should().Be(x);
        hashiPoint.Y.Should().Be(y);
        hashiPoint.PointType.Should().Be(pointType);
    }

    [Test]
    public void Constructor_WhenPointTypeNotProvided_ShouldDefaultToNormal()
    {
        // Arrange
        var x = 3;
        var y = 7;

        // Act
        var hashiPoint = new HashiPoint(x, y);

        // Assert
        hashiPoint.X.Should().Be(x);
        hashiPoint.Y.Should().Be(y);
        hashiPoint.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void Constructor_WhenNegativeCoordinates_ShouldAcceptValues()
    {
        // Arrange
        var x = -5;
        var y = -10;

        // Act
        var hashiPoint = new HashiPoint(x, y);

        // Assert
        hashiPoint.X.Should().Be(x);
        hashiPoint.Y.Should().Be(y);
    }

    [Test]
    public void Constructor_WhenZeroCoordinates_ShouldAcceptValues()
    {
        // Arrange
        var x = 0;
        var y = 0;

        // Act
        var hashiPoint = new HashiPoint(x, y);

        // Assert
        hashiPoint.X.Should().Be(x);
        hashiPoint.Y.Should().Be(y);
    }

    [Test]
    public void X_Property_ShouldBeReadOnly()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act
        var x = hashiPoint.X;

        // Assert
        x.Should().Be(5);
        // X property should not have a setter - this is verified at compile time
    }

    [Test]
    public void Y_Property_ShouldBeReadOnly()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act
        var y = hashiPoint.Y;

        // Assert
        y.Should().Be(10);
        // Y property should not have a setter - this is verified at compile time
    }

    [Test]
    public void PointType_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act
        hashiPoint.PointType = HashiPointTypeEnum.Hint;

        // Assert
        hashiPoint.PointType.Should().Be(HashiPointTypeEnum.Hint);
    }

    [Test]
    public void PointType_WhenChangedMultipleTimes_ShouldUpdateCorrectly()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act & Assert
        hashiPoint.PointType = HashiPointTypeEnum.Hint;
        hashiPoint.PointType.Should().Be(HashiPointTypeEnum.Hint);

        hashiPoint.PointType = HashiPointTypeEnum.Test;
        hashiPoint.PointType.Should().Be(HashiPointTypeEnum.Test);

        hashiPoint.PointType = HashiPointTypeEnum.All;
        hashiPoint.PointType.Should().Be(HashiPointTypeEnum.All);

        hashiPoint.PointType = HashiPointTypeEnum.Normal;
        hashiPoint.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    [Test]
    public void Clone_WhenCalled_ShouldReturnNewInstanceWithSameValues()
    {
        // Arrange
        var original = new HashiPoint(5, 10, HashiPointTypeEnum.Hint);

        // Act
        var clone = original.Clone();

        // Assert
        clone.Should().NotBeNull();
        clone.Should().NotBeSameAs(original);
        clone.Should().BeOfType<HashiPoint>();

        var clonedPoint = (HashiPoint)clone;
        clonedPoint.X.Should().Be(original.X);
        clonedPoint.Y.Should().Be(original.Y);
        clonedPoint.PointType.Should().Be(original.PointType);
    }

    [Test]
    public void Clone_WhenOriginalIsModified_ShouldNotAffectClone()
    {
        // Arrange
        var original = new HashiPoint(5, 10);
        var clone = (HashiPoint)original.Clone();

        // Act
        original.PointType = HashiPointTypeEnum.Hint;

        // Assert
        clone.PointType.Should().Be(HashiPointTypeEnum.Normal);
        original.PointType.Should().Be(HashiPointTypeEnum.Hint);
    }

    [Test]
    public void Clone_WhenCloneIsModified_ShouldNotAffectOriginal()
    {
        // Arrange
        var original = new HashiPoint(5, 10);
        var clone = (HashiPoint)original.Clone();

        // Act
        clone.PointType = HashiPointTypeEnum.Test;

        // Assert
        original.PointType.Should().Be(HashiPointTypeEnum.Normal);
        clone.PointType.Should().Be(HashiPointTypeEnum.Test);
    }

    [Test]
    public void ToString_WhenCalled_ShouldReturnFormattedString()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10, HashiPointTypeEnum.Hint);

        // Act
        var result = hashiPoint.ToString();

        // Assert
        result.Should().Be("Coordinate (X = 5, Y = 10), PointType = Hint");
    }

    [Test]
    public void ToString_WhenCoordinatesAreNegative_ShouldReturnFormattedString()
    {
        // Arrange
        var hashiPoint = new HashiPoint(-3, -7, HashiPointTypeEnum.Test);

        // Act
        var result = hashiPoint.ToString();

        // Assert
        result.Should().Be("Coordinate (X = -3, Y = -7), PointType = Test");
    }

    [Test]
    public void ToString_WhenCoordinatesAreZero_ShouldReturnFormattedString()
    {
        // Arrange
        var hashiPoint = new HashiPoint(0, 0);

        // Act
        var result = hashiPoint.ToString();

        // Assert
        result.Should().Be("Coordinate (X = 0, Y = 0), PointType = Normal");
    }

    [Test]
    public void HashiPoint_ShouldImplementIHashiPoint()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act & Assert
        hashiPoint.Should().BeAssignableTo<IHashiPoint>();
    }

    [Test]
    public void HashiPoint_ShouldImplementICloneable()
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act & Assert
        hashiPoint.Should().BeAssignableTo<ICloneable>();
    }

    [Test]
    [TestCase(HashiPointTypeEnum.Normal)]
    [TestCase(HashiPointTypeEnum.Hint)]
    [TestCase(HashiPointTypeEnum.Test)]
    [TestCase(HashiPointTypeEnum.All)]
    public void PointType_WhenSetToValidEnum_ShouldAcceptValue(HashiPointTypeEnum pointType)
    {
        // Arrange
        var hashiPoint = new HashiPoint(5, 10);

        // Act
        hashiPoint.PointType = pointType;

        // Assert
        hashiPoint.PointType.Should().Be(pointType);
    }

    [Test]
    public void Constructor_WhenLargeCoordinates_ShouldAcceptValues()
    {
        // Arrange
        var x = int.MaxValue;
        var y = int.MaxValue;

        // Act
        var hashiPoint = new HashiPoint(x, y);

        // Assert
        hashiPoint.X.Should().Be(x);
        hashiPoint.Y.Should().Be(y);
    }

    [Test]
    public void Constructor_WhenMinCoordinates_ShouldAcceptValues()
    {
        // Arrange
        var x = int.MinValue;
        var y = int.MinValue;

        // Act
        var hashiPoint = new HashiPoint(x, y);

        // Assert
        hashiPoint.X.Should().Be(x);
        hashiPoint.Y.Should().Be(y);
    }
}