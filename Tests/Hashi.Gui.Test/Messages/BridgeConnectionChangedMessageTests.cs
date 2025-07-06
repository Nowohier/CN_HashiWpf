using FluentAssertions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Messages;
using Moq;

namespace Hashi.Gui.Test.Messages;

[TestFixture]
public class BridgeConnectionChangedMessageTests
{
    private Mock<IBridgeConnectionInformationContainer> bridgeInfoMock;
    private BridgeConnectionChangedMessage bridgeConnectionChangedMessage;

    [SetUp]
    public void SetUp()
    {
        bridgeInfoMock = new Mock<IBridgeConnectionInformationContainer>(MockBehavior.Strict);
        bridgeConnectionChangedMessage = new BridgeConnectionChangedMessage(bridgeInfoMock.Object);
    }

    [Test]
    public void Constructor_WhenValidParameter_ShouldInitializeValue()
    {
        // Arrange
        var bridgeInfo = bridgeInfoMock.Object;

        // Act
        var result = new BridgeConnectionChangedMessage(bridgeInfo);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(bridgeInfo);
    }

    [Test]
    public void Constructor_WhenParameterIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new BridgeConnectionChangedMessage(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void BridgeConnectionChangedMessage_ShouldImplementIBridgeConnectionChangedMessage()
    {
        // Act & Assert
        bridgeConnectionChangedMessage.Should().BeAssignableTo<IBridgeConnectionChangedMessage>();
    }

    [Test]
    public void BridgeConnectionChangedMessage_ShouldInheritFromValueChangedMessage()
    {
        // Act & Assert
        bridgeConnectionChangedMessage.Should().BeAssignableTo<CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<IBridgeConnectionInformationContainer>>();
    }

    [Test]
    public void Value_WhenAccessed_ShouldReturnConstructorParameter()
    {
        // Act
        var result = bridgeConnectionChangedMessage.Value;

        // Assert
        result.Should().Be(bridgeInfoMock.Object);
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Arrange
        var anotherBridgeInfoMock = new Mock<IBridgeConnectionInformationContainer>(MockBehavior.Strict);

        // Act
        var message1 = new BridgeConnectionChangedMessage(bridgeInfoMock.Object);
        var message2 = new BridgeConnectionChangedMessage(anotherBridgeInfoMock.Object);

        // Assert
        message1.Should().NotBeSameAs(message2);
        message1.Value.Should().Be(bridgeInfoMock.Object);
        message2.Value.Should().Be(anotherBridgeInfoMock.Object);
    }

    [Test]
    public void BridgeConnectionChangedMessage_ShouldBePublicClass()
    {
        // Act & Assert
        var type = typeof(BridgeConnectionChangedMessage);
        type.IsPublic.Should().BeTrue();
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeFalse();
        type.IsSealed.Should().BeFalse();
    }

    [Test]
    public void Constructor_ShouldBePublic()
    {
        // Arrange
        var constructor = typeof(BridgeConnectionChangedMessage).GetConstructor(new[] { typeof(IBridgeConnectionInformationContainer) });

        // Assert
        constructor.Should().NotBeNull();
        constructor!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void BridgeConnectionChangedMessage_ShouldInheritFromCorrectBaseClass()
    {
        // Arrange
        var type = typeof(BridgeConnectionChangedMessage);

        // Act
        var baseType = type.BaseType;

        // Assert
        baseType.Should().NotBeNull();
        baseType!.IsGenericType.Should().BeTrue();
        baseType.GetGenericTypeDefinition().Should().Be(typeof(CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<>));
        baseType.GetGenericArguments().Should().HaveCount(1);
        baseType.GetGenericArguments()[0].Should().Be(typeof(IBridgeConnectionInformationContainer));
    }

    [Test]
    public void BridgeConnectionChangedMessage_ShouldImplementCorrectInterface()
    {
        // Arrange
        var type = typeof(BridgeConnectionChangedMessage);

        // Act
        var interfaces = type.GetInterfaces();

        // Assert
        interfaces.Should().Contain(typeof(IBridgeConnectionChangedMessage));
    }

    [Test]
    public void Value_Property_ShouldBeInherited()
    {
        // Arrange & Act
        var property = bridgeConnectionChangedMessage.GetType().GetProperty("Value");

        // Assert
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(IBridgeConnectionInformationContainer));
        property.CanRead.Should().BeTrue();
        property.CanWrite.Should().BeFalse(); // Value is typically read-only in ValueChangedMessage
    }

    [Test]
    public void Constructor_WhenDifferentBridgeInfoInstances_ShouldStoreCorrectReference()
    {
        // Arrange
        var bridgeInfo1 = new Mock<IBridgeConnectionInformationContainer>(MockBehavior.Strict);
        var bridgeInfo2 = new Mock<IBridgeConnectionInformationContainer>(MockBehavior.Strict);

        // Act
        var message1 = new BridgeConnectionChangedMessage(bridgeInfo1.Object);
        var message2 = new BridgeConnectionChangedMessage(bridgeInfo2.Object);

        // Assert
        message1.Value.Should().BeSameAs(bridgeInfo1.Object);
        message2.Value.Should().BeSameAs(bridgeInfo2.Object);
        message1.Value.Should().NotBeSameAs(message2.Value);
    }

    [Test]
    public void Constructor_WithSameBridgeInfoInstance_ShouldStoreCorrectReference()
    {
        // Arrange
        var sharedBridgeInfo = bridgeInfoMock.Object;

        // Act
        var message1 = new BridgeConnectionChangedMessage(sharedBridgeInfo);
        var message2 = new BridgeConnectionChangedMessage(sharedBridgeInfo);

        // Assert
        message1.Value.Should().BeSameAs(sharedBridgeInfo);
        message2.Value.Should().BeSameAs(sharedBridgeInfo);
        message1.Value.Should().BeSameAs(message2.Value);
        message1.Should().NotBeSameAs(message2); // Different message instances
    }

    [Test]
    public void Constructor_ParameterName_ShouldBeCorrect()
    {
        // Arrange
        var constructor = typeof(BridgeConnectionChangedMessage).GetConstructor(new[] { typeof(IBridgeConnectionInformationContainer) });

        // Act
        var parameters = constructor!.GetParameters();

        // Assert
        parameters.Should().HaveCount(1);
        parameters[0].Name.Should().Be("islandInfos");
        parameters[0].ParameterType.Should().Be(typeof(IBridgeConnectionInformationContainer));
    }

    [Test]
    public void BridgeConnectionChangedMessage_ShouldHaveCorrectNamespace()
    {
        // Act & Assert
        var type = typeof(BridgeConnectionChangedMessage);
        type.Namespace.Should().Be("Hashi.Gui.Messages");
    }

    [Test]
    public void Constructor_WhenCalledWithValidBridgeInfo_ShouldNotModifyBridgeInfo()
    {
        // Arrange
        var originalBridgeInfo = bridgeInfoMock.Object;

        // Act
        var message = new BridgeConnectionChangedMessage(originalBridgeInfo);

        // Assert
        message.Value.Should().BeSameAs(originalBridgeInfo);
        // No verification needed on mock as we're not calling any methods on it
    }
}