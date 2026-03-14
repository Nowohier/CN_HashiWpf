using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Core.Models;
using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Test.Models;

/// <summary>
/// Unit tests for <see cref="BasicHashiPoint"/> class.
/// </summary>
[TestFixture]
public class BasicHashiPointTests
{
    #region Constructor Tests

    [Test]
    public void Constructor_WhenValidParameters_ShouldSetProperties()
    {
        // Arrange & Act
        var point = new BasicHashiPoint(3, 5, HashiPointTypeEnum.Hint);

        // Assert
        point.X.Should().Be(3);
        point.Y.Should().Be(5);
        point.PointType.Should().Be(HashiPointTypeEnum.Hint);
    }

    [Test]
    public void Constructor_WhenDefaultPointType_ShouldBeNormal()
    {
        // Arrange & Act
        var point = new BasicHashiPoint(1, 2);

        // Assert
        point.PointType.Should().Be(HashiPointTypeEnum.Normal);
    }

    #endregion

    #region Clone Tests

    [Test]
    public void Clone_WhenCalled_ShouldReturnNewInstanceWithSameValues()
    {
        // Arrange
        var original = new BasicHashiPoint(4, 7, HashiPointTypeEnum.Test);

        // Act
        var clone = (BasicHashiPoint)original.Clone();

        // Assert
        clone.X.Should().Be(original.X);
        clone.Y.Should().Be(original.Y);
        clone.PointType.Should().Be(original.PointType);
    }

    [Test]
    public void Clone_WhenCalled_ShouldNotReturnSameReference()
    {
        // Arrange
        var original = new BasicHashiPoint(4, 7, HashiPointTypeEnum.Test);

        // Act
        var clone = original.Clone();

        // Assert
        clone.Should().NotBeSameAs(original);
    }

    #endregion

    #region Equals Tests

    [Test]
    public void Equals_WhenSameCoordinatesAndType_ShouldReturnTrue()
    {
        // Arrange
        var point1 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Hint);
        var point2 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Hint);

        // Act & Assert
        point1.Equals(point2).Should().BeTrue();
    }

    [Test]
    public void Equals_WhenDifferentX_ShouldReturnFalse()
    {
        // Arrange
        var point1 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Normal);
        var point2 = new BasicHashiPoint(5, 3, HashiPointTypeEnum.Normal);

        // Act & Assert
        point1.Equals(point2).Should().BeFalse();
    }

    [Test]
    public void Equals_WhenDifferentY_ShouldReturnFalse()
    {
        // Arrange
        var point1 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Normal);
        var point2 = new BasicHashiPoint(2, 8, HashiPointTypeEnum.Normal);

        // Act & Assert
        point1.Equals(point2).Should().BeFalse();
    }

    [Test]
    public void Equals_WhenDifferentPointType_ShouldReturnFalse()
    {
        // Arrange
        var point1 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Normal);
        var point2 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Hint);

        // Act & Assert
        point1.Equals(point2).Should().BeFalse();
    }

    [Test]
    public void Equals_WhenNull_ShouldReturnFalse()
    {
        // Arrange
        var point = new BasicHashiPoint(2, 3);

        // Act & Assert
        point.Equals(null).Should().BeFalse();
    }

    [Test]
    public void Equals_WhenDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var point = new BasicHashiPoint(2, 3);

        // Act & Assert
        point.Equals("not a point").Should().BeFalse();
    }

    #endregion

    #region GetHashCode Tests

    [Test]
    public void GetHashCode_WhenSameValues_ShouldReturnSameHash()
    {
        // Arrange
        var point1 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Hint);
        var point2 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Hint);

        // Act & Assert
        point1.GetHashCode().Should().Be(point2.GetHashCode());
    }

    [Test]
    public void GetHashCode_WhenDifferentValues_ShouldReturnDifferentHash()
    {
        // Arrange
        var point1 = new BasicHashiPoint(2, 3, HashiPointTypeEnum.Normal);
        var point2 = new BasicHashiPoint(5, 8, HashiPointTypeEnum.Hint);

        // Act & Assert
        point1.GetHashCode().Should().NotBe(point2.GetHashCode());
    }

    #endregion
}
