using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Messages.MessageContainers;
using Moq;

namespace Hashi.Gui.Test.Messages.MessageContainers;

[TestFixture]
public class BridgeConnectionInformationContainerTests
{
    private Mock<IIslandViewModel> sourceIslandMock;
    private Mock<IIslandViewModel> targetIslandMock;
    private BridgeConnectionInformationContainer container;

    [SetUp]
    public void SetUp()
    {
        sourceIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        targetIslandMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        
        container = new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.Add,
            sourceIslandMock.Object,
            targetIslandMock.Object);
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        var operationType = BridgeOperationTypeEnum.Remove;
        var sourceIsland = sourceIslandMock.Object;
        var targetIsland = targetIslandMock.Object;

        // Act
        var result = new BridgeConnectionInformationContainer(operationType, sourceIsland, targetIsland);

        // Assert
        result.BridgeOperationType.Should().Be(operationType);
        result.SourceIsland.Should().Be(sourceIsland);
        result.TargetIsland.Should().Be(targetIsland);
    }

    [Test]
    public void Constructor_WhenSourceIslandIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.Add,
            null!,
            targetIslandMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenTargetIslandIsNull_ShouldAllowNullTargetIsland()
    {
        // Act
        var result = new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.Add,
            sourceIslandMock.Object,
            null);

        // Assert
        result.Should().NotBeNull();
        result.SourceIsland.Should().Be(sourceIslandMock.Object);
        result.TargetIsland.Should().BeNull();
        result.BridgeOperationType.Should().Be(BridgeOperationTypeEnum.Add);
    }

    [Test]
    public void Constructor_WhenTargetIslandNotProvided_ShouldDefaultToNull()
    {
        // Act
        var result = new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.RemoveAll,
            sourceIslandMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.SourceIsland.Should().Be(sourceIslandMock.Object);
        result.TargetIsland.Should().BeNull();
        result.BridgeOperationType.Should().Be(BridgeOperationTypeEnum.RemoveAll);
    }

    [Test]
    public void BridgeOperationType_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Arrange
        var expectedType = BridgeOperationTypeEnum.Add;

        // Act
        var result = container.BridgeOperationType;

        // Assert
        result.Should().Be(expectedType);
    }

    [Test]
    public void SourceIsland_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Act
        var result = container.SourceIsland;

        // Assert
        result.Should().Be(sourceIslandMock.Object);
    }

    [Test]
    public void TargetIsland_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Act
        var result = container.TargetIsland;

        // Assert
        result.Should().Be(targetIslandMock.Object);
    }

    [Test]
    public void BridgeConnectionInformationContainer_ShouldImplementIBridgeConnectionInformationContainer()
    {
        // Act & Assert
        container.Should().BeAssignableTo<IBridgeConnectionInformationContainer>();
    }

    [Test]
    [TestCase(BridgeOperationTypeEnum.Add)]
    [TestCase(BridgeOperationTypeEnum.Remove)]
    [TestCase(BridgeOperationTypeEnum.RemoveAll)]
    public void Constructor_WhenDifferentOperationTypes_ShouldSetOperationTypeCorrectly(BridgeOperationTypeEnum operationType)
    {
        // Act
        var result = new BridgeConnectionInformationContainer(operationType, sourceIslandMock.Object, targetIslandMock.Object);

        // Assert
        result.BridgeOperationType.Should().Be(operationType);
    }

    [Test]
    public void Properties_WhenAccessed_ShouldBeReadOnly()
    {
        // Act & Assert
        var bridgeOperationTypeProperty = typeof(BridgeConnectionInformationContainer).GetProperty(nameof(BridgeConnectionInformationContainer.BridgeOperationType));
        var sourceIslandProperty = typeof(BridgeConnectionInformationContainer).GetProperty(nameof(BridgeConnectionInformationContainer.SourceIsland));
        var targetIslandProperty = typeof(BridgeConnectionInformationContainer).GetProperty(nameof(BridgeConnectionInformationContainer.TargetIsland));

        bridgeOperationTypeProperty.Should().NotBeNull();
        sourceIslandProperty.Should().NotBeNull();
        targetIslandProperty.Should().NotBeNull();

        bridgeOperationTypeProperty!.CanWrite.Should().BeFalse();
        sourceIslandProperty!.CanWrite.Should().BeFalse();
        targetIslandProperty!.CanWrite.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Arrange
        var anotherSourceMock = new Mock<IIslandViewModel>(MockBehavior.Strict);
        var anotherTargetMock = new Mock<IIslandViewModel>(MockBehavior.Strict);

        // Act
        var container1 = new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.Add,
            sourceIslandMock.Object,
            targetIslandMock.Object);

        var container2 = new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.Remove,
            anotherSourceMock.Object,
            anotherTargetMock.Object);

        // Assert
        container1.Should().NotBeSameAs(container2);
        container1.BridgeOperationType.Should().NotBe(container2.BridgeOperationType);
        container1.SourceIsland.Should().NotBe(container2.SourceIsland);
        container1.TargetIsland.Should().NotBe(container2.TargetIsland);
    }

    [Test]
    public void Constructor_WhenSameIslandUsedForBothSourceAndTarget_ShouldAcceptSameIsland()
    {
        // Act
        var result = new BridgeConnectionInformationContainer(
            BridgeOperationTypeEnum.Add,
            sourceIslandMock.Object,
            sourceIslandMock.Object);

        // Assert
        result.SourceIsland.Should().Be(sourceIslandMock.Object);
        result.TargetIsland.Should().Be(sourceIslandMock.Object);
        result.SourceIsland.Should().BeSameAs(result.TargetIsland);
    }

    [Test]
    public void BridgeConnectionInformationContainer_ShouldBePublicClass()
    {
        // Act & Assert
        var type = typeof(BridgeConnectionInformationContainer);
        type.IsPublic.Should().BeTrue();
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeFalse();
        type.IsSealed.Should().BeFalse();
    }

    [Test]
    public void Constructor_ShouldBePublic()
    {
        // Arrange
        var constructorWithTarget = typeof(BridgeConnectionInformationContainer).GetConstructor(
            new[] { typeof(BridgeOperationTypeEnum), typeof(IIslandViewModel), typeof(IIslandViewModel) });

        // Assert
        constructorWithTarget.Should().NotBeNull();
        constructorWithTarget!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void Constructor_ParameterNames_ShouldBeCorrect()
    {
        // Arrange
        var constructor = typeof(BridgeConnectionInformationContainer).GetConstructor(
            new[] { typeof(BridgeOperationTypeEnum), typeof(IIslandViewModel), typeof(IIslandViewModel) });

        // Act
        var parameters = constructor!.GetParameters();

        // Assert
        parameters.Should().HaveCount(3);
        parameters[0].Name.Should().Be("bridgeOperationType");
        parameters[1].Name.Should().Be("sourceIsland");
        parameters[2].Name.Should().Be("targetIsland");
    }

    [Test]
    public void Constructor_WithOptionalTargetParameter_ShouldHaveDefaultValue()
    {
        // Arrange
        var constructor = typeof(BridgeConnectionInformationContainer).GetConstructor(
            new[] { typeof(BridgeOperationTypeEnum), typeof(IIslandViewModel), typeof(IIslandViewModel) });

        // Act
        var parameters = constructor!.GetParameters();

        // Assert
        parameters[2].HasDefaultValue.Should().BeTrue();
        parameters[2].DefaultValue.Should().BeNull();
    }

    [Test]
    public void BridgeConnectionInformationContainer_ShouldHaveCorrectNamespace()
    {
        // Act & Assert
        var type = typeof(BridgeConnectionInformationContainer);
        type.Namespace.Should().Be("Hashi.Gui.Messages.MessageContainers");
    }

    [Test]
    public void Constructor_WhenInvalidOperationType_ShouldStillInitialize()
    {
        // Arrange
        var invalidOperationType = (BridgeOperationTypeEnum)999;

        // Act
        var result = new BridgeConnectionInformationContainer(
            invalidOperationType,
            sourceIslandMock.Object,
            targetIslandMock.Object);

        // Assert
        result.BridgeOperationType.Should().Be(invalidOperationType);
        result.SourceIsland.Should().Be(sourceIslandMock.Object);
        result.TargetIsland.Should().Be(targetIslandMock.Object);
    }

    [Test]
    public void AllProperties_ShouldImplementInterfaceProperties()
    {
        // Arrange
        var interfaceType = typeof(IBridgeConnectionInformationContainer);
        var implementationType = typeof(BridgeConnectionInformationContainer);

        // Act & Assert
        var interfaceProperties = interfaceType.GetProperties();
        
        foreach (var interfaceProperty in interfaceProperties)
        {
            var implementationProperty = implementationType.GetProperty(interfaceProperty.Name);
            implementationProperty.Should().NotBeNull($"Property {interfaceProperty.Name} should be implemented");
            implementationProperty!.PropertyType.Should().Be(interfaceProperty.PropertyType);
        }
    }
}