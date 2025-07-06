using FluentAssertions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Messages;

namespace Hashi.Gui.Test.Messages;

[TestFixture]
public class AllConnectionsSetMessageTests
{
    [Test]
    public void Constructor_WhenCalledWithTrue_ShouldInitializeValue()
    {
        // Arrange
        var value = true;

        // Act
        var result = new AllConnectionsSetMessage(value);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(value);
    }

    [Test]
    public void Constructor_WhenCalledWithFalse_ShouldInitializeValue()
    {
        // Arrange
        var value = false;

        // Act
        var result = new AllConnectionsSetMessage(value);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(value);
    }

    [Test]
    public void Constructor_WhenCalledWithNull_ShouldInitializeValue()
    {
        // Arrange
        bool? value = null;

        // Act
        var result = new AllConnectionsSetMessage(value);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeNull();
    }

    [Test]
    public void AllConnectionsSetMessage_ShouldImplementIAllConnectionsSetMessage()
    {
        // Arrange
        var message = new AllConnectionsSetMessage(true);

        // Act & Assert
        message.Should().BeAssignableTo<IAllConnectionsSetMessage>();
    }

    [Test]
    public void AllConnectionsSetMessage_ShouldInheritFromValueChangedMessage()
    {
        // Arrange
        var message = new AllConnectionsSetMessage(true);

        // Act & Assert
        message.Should().BeAssignableTo<CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<bool?>>();
    }

    [Test]
    public void Value_WhenAccessed_ShouldReturnConstructorValue()
    {
        // Arrange
        var expectedValue = true;
        var message = new AllConnectionsSetMessage(expectedValue);

        // Act
        var result = message.Value;

        // Assert
        result.Should().Be(expectedValue);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void Constructor_WhenCalledWithDifferentValues_ShouldStoreValueCorrectly(bool? testValue)
    {
        // Act
        var result = new AllConnectionsSetMessage(testValue);

        // Assert
        result.Value.Should().Be(testValue);
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Arrange
        var value1 = true;
        var value2 = false;

        // Act
        var message1 = new AllConnectionsSetMessage(value1);
        var message2 = new AllConnectionsSetMessage(value2);

        // Assert
        message1.Should().NotBeSameAs(message2);
        message1.Value.Should().Be(value1);
        message2.Value.Should().Be(value2);
    }

    [Test]
    public void AllConnectionsSetMessage_ShouldBePublicClass()
    {
        // Act & Assert
        var type = typeof(AllConnectionsSetMessage);
        type.IsPublic.Should().BeTrue();
        type.IsClass.Should().BeTrue();
        type.IsAbstract.Should().BeFalse();
        type.IsSealed.Should().BeFalse();
    }

    [Test]
    public void Constructor_ShouldBePublic()
    {
        // Arrange
        var constructor = typeof(AllConnectionsSetMessage).GetConstructor(new[] { typeof(bool?) });

        // Assert
        constructor.Should().NotBeNull();
        constructor!.IsPublic.Should().BeTrue();
    }

    [Test]
    public void AllConnectionsSetMessage_ShouldInheritFromCorrectBaseClass()
    {
        // Arrange
        var type = typeof(AllConnectionsSetMessage);

        // Act
        var baseType = type.BaseType;

        // Assert
        baseType.Should().NotBeNull();
        baseType!.IsGenericType.Should().BeTrue();
        baseType.GetGenericTypeDefinition().Should().Be(typeof(CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<>));
        baseType.GetGenericArguments().Should().HaveCount(1);
        baseType.GetGenericArguments()[0].Should().Be(typeof(bool?));
    }

    [Test]
    public void AllConnectionsSetMessage_ShouldImplementCorrectInterface()
    {
        // Arrange
        var type = typeof(AllConnectionsSetMessage);

        // Act
        var interfaces = type.GetInterfaces();

        // Assert
        interfaces.Should().Contain(typeof(IAllConnectionsSetMessage));
    }

    [Test]
    public void Value_Property_ShouldBeInherited()
    {
        // Arrange
        var message = new AllConnectionsSetMessage(true);

        // Act
        var property = message.GetType().GetProperty("Value");

        // Assert
        property.Should().NotBeNull();
        property!.PropertyType.Should().Be(typeof(bool?));
        property.CanRead.Should().BeTrue();
        property.CanWrite.Should().BeFalse(); // Value is typically read-only in ValueChangedMessage
    }

    [Test]
    public void Constructor_WhenCalledWithNullableBooleanTrue_ShouldMaintainType()
    {
        // Arrange
        bool? value = true;

        // Act
        var result = new AllConnectionsSetMessage(value);

        // Assert
        result.Value.Should().BeOfType(typeof(bool));
        result.Value.Should().Be(true);
    }

    [Test]
    public void Constructor_WhenCalledWithNullableBooleanFalse_ShouldMaintainType()
    {
        // Arrange
        bool? value = false;

        // Act
        var result = new AllConnectionsSetMessage(value);

        // Assert
        result.Value.Should().BeOfType(typeof(bool));
        result.Value.Should().Be(false);
    }
}