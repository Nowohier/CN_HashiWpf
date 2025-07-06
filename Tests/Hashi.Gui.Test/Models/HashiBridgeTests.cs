using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Models;
using Moq;

namespace Hashi.Gui.Test.Models;

[TestFixture]
public class HashiBridgeTests
{
    private Mock<IHashiPoint> point1Mock;
    private Mock<IHashiPoint> point2Mock;
    private HashiBridge hashiBridge;

    [SetUp]
    public void SetUp()
    {
        point1Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        point2Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        
        hashiBridge = new HashiBridge(BridgeOperationTypeEnum.Add, point1Mock.Object, point2Mock.Object);
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        var actionType = BridgeOperationTypeEnum.Remove;
        var point1 = point1Mock.Object;
        var point2 = point2Mock.Object;

        // Act
        var result = new HashiBridge(actionType, point1, point2);

        // Assert
        result.ActionType.Should().Be(actionType);
        result.Point1.Should().Be(point1);
        result.Point2.Should().Be(point2);
    }

    [Test]
    public void Constructor_WhenPoint1IsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HashiBridge(BridgeOperationTypeEnum.Add, null!, point2Mock.Object);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenPoint2IsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new HashiBridge(BridgeOperationTypeEnum.Add, point1Mock.Object, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ActionType_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Arrange
        var expectedActionType = BridgeOperationTypeEnum.Add;

        // Act
        var result = hashiBridge.ActionType;

        // Assert
        result.Should().Be(expectedActionType);
    }

    [Test]
    public void Point1_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Act
        var result = hashiBridge.Point1;

        // Assert
        result.Should().Be(point1Mock.Object);
    }

    [Test]
    public void Point2_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Act
        var result = hashiBridge.Point2;

        // Assert
        result.Should().Be(point2Mock.Object);
    }

    [Test]
    public void HashiBridge_ShouldImplementIHashiBridge()
    {
        // Act & Assert
        hashiBridge.Should().BeAssignableTo<IHashiBridge>();
    }

    [Test]
    [TestCase(BridgeOperationTypeEnum.Add)]
    [TestCase(BridgeOperationTypeEnum.Remove)]
    [TestCase(BridgeOperationTypeEnum.RemoveAll)]
    public void Constructor_WhenDifferentActionTypes_ShouldSetActionTypeCorrectly(BridgeOperationTypeEnum actionType)
    {
        // Act
        var result = new HashiBridge(actionType, point1Mock.Object, point2Mock.Object);

        // Assert
        result.ActionType.Should().Be(actionType);
    }

    [Test]
    public void Properties_WhenAccessed_ShouldBeReadOnly()
    {
        // Act & Assert
        var actionTypeProperty = typeof(HashiBridge).GetProperty(nameof(HashiBridge.ActionType));
        var point1Property = typeof(HashiBridge).GetProperty(nameof(HashiBridge.Point1));
        var point2Property = typeof(HashiBridge).GetProperty(nameof(HashiBridge.Point2));

        actionTypeProperty.Should().NotBeNull();
        point1Property.Should().NotBeNull();
        point2Property.Should().NotBeNull();

        actionTypeProperty!.CanWrite.Should().BeFalse();
        point1Property!.CanWrite.Should().BeFalse();
        point2Property!.CanWrite.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var bridge1 = new HashiBridge(BridgeOperationTypeEnum.Add, point1Mock.Object, point2Mock.Object);
        var bridge2 = new HashiBridge(BridgeOperationTypeEnum.Remove, point1Mock.Object, point2Mock.Object);

        // Assert
        bridge1.Should().NotBeSameAs(bridge2);
        bridge1.ActionType.Should().NotBe(bridge2.ActionType);
        bridge1.Point1.Should().Be(bridge2.Point1);
        bridge1.Point2.Should().Be(bridge2.Point2);
    }

    [Test]
    public void Constructor_WhenSamePointsButDifferentInstances_ShouldAcceptDifferentInstances()
    {
        // Arrange
        var anotherPoint1Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var anotherPoint2Mock = new Mock<IHashiPoint>(MockBehavior.Strict);

        // Act
        var result = new HashiBridge(BridgeOperationTypeEnum.Add, anotherPoint1Mock.Object, anotherPoint2Mock.Object);

        // Assert
        result.Point1.Should().Be(anotherPoint1Mock.Object);
        result.Point2.Should().Be(anotherPoint2Mock.Object);
        result.Point1.Should().NotBe(point1Mock.Object);
        result.Point2.Should().NotBe(point2Mock.Object);
    }

    [Test]
    public void Constructor_WhenSamePointUsedForBothPoints_ShouldAcceptSamePoint()
    {
        // Act
        var result = new HashiBridge(BridgeOperationTypeEnum.Add, point1Mock.Object, point1Mock.Object);

        // Assert
        result.Point1.Should().Be(point1Mock.Object);
        result.Point2.Should().Be(point1Mock.Object);
        result.Point1.Should().BeSameAs(result.Point2);
    }

    [Test]
    public void Constructor_WhenInvalidActionType_ShouldStillInitialize()
    {
        // Arrange
        var invalidActionType = (BridgeOperationTypeEnum)999;

        // Act
        var result = new HashiBridge(invalidActionType, point1Mock.Object, point2Mock.Object);

        // Assert
        result.ActionType.Should().Be(invalidActionType);
        result.Point1.Should().Be(point1Mock.Object);
        result.Point2.Should().Be(point2Mock.Object);
    }
}