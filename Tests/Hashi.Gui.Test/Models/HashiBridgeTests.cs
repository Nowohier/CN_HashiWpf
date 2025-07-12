using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Models;
using Hashi.Gui.Models;
using Moq;

namespace Hashi.Gui.Test.Models;

[TestFixture]
public class HashiBridgeTests
{
    private HashiBridge hashiBridge;

    [SetUp]
    public void SetUp()
    {
        var point1Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var point2Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        
        point1Mock.Setup(p => p.X).Returns(1);
        point1Mock.Setup(p => p.Y).Returns(1);
        point2Mock.Setup(p => p.X).Returns(2);
        point2Mock.Setup(p => p.Y).Returns(2);
        
        hashiBridge = new HashiBridge(BridgeOperationTypeEnum.Add, point1Mock.Object, point2Mock.Object);
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalledWithValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        var point1Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var point2Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        
        point1Mock.Setup(p => p.X).Returns(3);
        point1Mock.Setup(p => p.Y).Returns(4);
        point2Mock.Setup(p => p.X).Returns(5);
        point2Mock.Setup(p => p.Y).Returns(6);

        // Act
        var result = new HashiBridge(BridgeOperationTypeEnum.Remove, point1Mock.Object, point2Mock.Object);

        // Assert
        result.ActionType.Should().Be(BridgeOperationTypeEnum.Remove);
        result.Point1.Should().Be(point1Mock.Object);
        result.Point2.Should().Be(point2Mock.Object);
    }

    [Test]
    public void Constructor_WhenCalledWithNullPoint1_ShouldAllowNullValue()
    {
        // Arrange
        var point2Mock = new Mock<IHashiPoint>(MockBehavior.Strict);

        // Act
        var result = new HashiBridge(BridgeOperationTypeEnum.Add, null!, point2Mock.Object);

        // Assert
        result.ActionType.Should().Be(BridgeOperationTypeEnum.Add);
        result.Point1.Should().BeNull();
        result.Point2.Should().Be(point2Mock.Object);
    }

    [Test]
    public void Constructor_WhenCalledWithNullPoint2_ShouldAllowNullValue()
    {
        // Arrange
        var point1Mock = new Mock<IHashiPoint>(MockBehavior.Strict);

        // Act
        var result = new HashiBridge(BridgeOperationTypeEnum.Add, point1Mock.Object, null!);

        // Assert
        result.ActionType.Should().Be(BridgeOperationTypeEnum.Add);
        result.Point1.Should().Be(point1Mock.Object);
        result.Point2.Should().BeNull();
    }

    [Test]
    public void Constructor_WhenCalledWithBothPointsNull_ShouldAllowNullValues()
    {
        // Act
        var result = new HashiBridge(BridgeOperationTypeEnum.Add, null!, null!);

        // Assert
        result.ActionType.Should().Be(BridgeOperationTypeEnum.Add);
        result.Point1.Should().BeNull();
        result.Point2.Should().BeNull();
    }

    #endregion

    #region Property Tests

    [Test]
    public void ActionType_ShouldReturnCorrectValue()
    {
        // Act & Assert
        hashiBridge.ActionType.Should().Be(BridgeOperationTypeEnum.Add);
    }

    [Test]
    public void Point1_ShouldReturnCorrectValue()
    {
        // Act & Assert
        hashiBridge.Point1.Should().NotBeNull();
        hashiBridge.Point1.X.Should().Be(1);
        hashiBridge.Point1.Y.Should().Be(1);
    }

    [Test]
    public void Point2_ShouldReturnCorrectValue()
    {
        // Act & Assert
        hashiBridge.Point2.Should().NotBeNull();
        hashiBridge.Point2.X.Should().Be(2);
        hashiBridge.Point2.Y.Should().Be(2);
    }

    #endregion

    #region Different Operation Types Tests

    [Test]
    public void Constructor_WhenCalledWithAllOperationTypes_ShouldAcceptAllValidTypes()
    {
        // Arrange
        var point1Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        var point2Mock = new Mock<IHashiPoint>(MockBehavior.Strict);
        
        var operationTypes = Enum.GetValues<BridgeOperationTypeEnum>();

        // Act & Assert
        foreach (var operationType in operationTypes)
        {
            var bridge = new HashiBridge(operationType, point1Mock.Object, point2Mock.Object);
            bridge.ActionType.Should().Be(operationType);
        }
    }

    #endregion
}