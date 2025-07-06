using FluentAssertions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Providers;
using Moq;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class RuleInfoProviderTests
{
    private RuleInfoProvider ruleInfoProvider;
    private Mock<Func<bool?, IUpdateAllIslandColorsMessage>> updateIslandColorsMessageFactoryMock;
    private Mock<Func<bool?, IRuleMessageClearedMessage>> ruleMessageClearedMessageFactoryMock;
    private Mock<IUpdateAllIslandColorsMessage> updateIslandColorsMessageMock;
    private Mock<IRuleMessageClearedMessage> ruleMessageClearedMessageMock;

    [SetUp]
    public void SetUp()
    {
        updateIslandColorsMessageFactoryMock = new Mock<Func<bool?, IUpdateAllIslandColorsMessage>>(MockBehavior.Strict);
        ruleMessageClearedMessageFactoryMock = new Mock<Func<bool?, IRuleMessageClearedMessage>>(MockBehavior.Strict);
        updateIslandColorsMessageMock = new Mock<IUpdateAllIslandColorsMessage>(MockBehavior.Strict);
        ruleMessageClearedMessageMock = new Mock<IRuleMessageClearedMessage>(MockBehavior.Strict);

        updateIslandColorsMessageFactoryMock.Setup(x => x.Invoke(It.IsAny<bool?>()))
            .Returns(updateIslandColorsMessageMock.Object);
        ruleMessageClearedMessageFactoryMock.Setup(x => x.Invoke(It.IsAny<bool?>()))
            .Returns(ruleMessageClearedMessageMock.Object);

        ruleInfoProvider = new RuleInfoProvider(
            updateIslandColorsMessageFactoryMock.Object,
            ruleMessageClearedMessageFactoryMock.Object);
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Act
        var result = new RuleInfoProvider(
            updateIslandColorsMessageFactoryMock.Object,
            ruleMessageClearedMessageFactoryMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.AreRulesBeingApplied.Should().BeFalse();
        result.RuleMessage.Should().Be(string.Empty);
    }

    [Test]
    public void Constructor_WhenUpdateIslandColorsMessageFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new RuleInfoProvider(
            null!,
            ruleMessageClearedMessageFactoryMock.Object);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void Constructor_WhenRuleMessageClearedMessageFactoryIsNull_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var act = () => new RuleInfoProvider(
            updateIslandColorsMessageFactoryMock.Object,
            null!);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AreRulesBeingApplied_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newValue = true;

        // Act
        ruleInfoProvider.AreRulesBeingApplied = newValue;

        // Assert
        ruleInfoProvider.AreRulesBeingApplied.Should().Be(newValue);
    }

    [Test]
    public void RuleMessage_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newMessage = "Test rule message";

        // Act
        ruleInfoProvider.RuleMessage = newMessage;

        // Assert
        ruleInfoProvider.RuleMessage.Should().Be(newMessage);
    }

    [Test]
    public void RuleMessage_WhenSetToEmptyString_ShouldSendMessages()
    {
        // Arrange
        ruleInfoProvider.RuleMessage = "Some message";

        // Act
        ruleInfoProvider.RuleMessage = string.Empty;

        // Assert
        ruleInfoProvider.RuleMessage.Should().Be(string.Empty);
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
    }

    [Test]
    public void RuleMessage_WhenSetToSameValue_ShouldNotSendMessages()
    {
        // Arrange
        var message = "Test message";
        ruleInfoProvider.RuleMessage = message;
        
        // Reset mock invocations
        updateIslandColorsMessageFactoryMock.Reset();
        ruleMessageClearedMessageFactoryMock.Reset();

        // Act
        ruleInfoProvider.RuleMessage = message;

        // Assert
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<bool?>()), Times.Never);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<bool?>()), Times.Never);
    }

    [Test]
    public void RuleMessage_WhenSetToNonEmptyString_ShouldNotSendMessages()
    {
        // Arrange
        var message = "Test message";

        // Act
        ruleInfoProvider.RuleMessage = message;

        // Assert
        ruleInfoProvider.RuleMessage.Should().Be(message);
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<bool?>()), Times.Never);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<bool?>()), Times.Never);
    }

    [Test]
    public void RuleMessage_WhenSetFromNonEmptyToEmpty_ShouldSendMessages()
    {
        // Arrange
        ruleInfoProvider.RuleMessage = "Some message";

        // Act
        ruleInfoProvider.RuleMessage = string.Empty;

        // Assert
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
    }

    [Test]
    public void RuleMessage_WhenSetFromEmptyToNonEmpty_ShouldNotSendMessages()
    {
        // Arrange
        ruleInfoProvider.RuleMessage = string.Empty;
        
        // Reset mock invocations
        updateIslandColorsMessageFactoryMock.Reset();
        ruleMessageClearedMessageFactoryMock.Reset();

        // Act
        ruleInfoProvider.RuleMessage = "New message";

        // Assert
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<bool?>()), Times.Never);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(It.IsAny<bool?>()), Times.Never);
    }

    [Test]
    public void RuleInfoProvider_ShouldImplementIRuleInfoProvider()
    {
        // Act & Assert
        ruleInfoProvider.Should().BeAssignableTo<IRuleInfoProvider>();
    }

    [Test]
    public void RuleMessage_WhenSetMultipleTimes_ShouldOnlySendMessagesWhenChangingToEmpty()
    {
        // Arrange
        var messages = new[] { "Message 1", "Message 2", "Message 3", string.Empty };

        // Act
        foreach (var message in messages)
        {
            ruleInfoProvider.RuleMessage = message;
        }

        // Assert
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
    }

    [Test]
    public void RuleMessage_WhenSetToNull_ShouldSetToEmptyAndSendMessages()
    {
        // Arrange
        ruleInfoProvider.RuleMessage = "Some message";

        // Act
        ruleInfoProvider.RuleMessage = null!;

        // Assert
        ruleInfoProvider.RuleMessage.Should().BeNull();
        updateIslandColorsMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
        ruleMessageClearedMessageFactoryMock.Verify(x => x.Invoke(null), Times.Once);
    }

    [Test]
    public void AreRulesBeingApplied_WhenToggledMultipleTimes_ShouldMaintainCorrectState()
    {
        // Act & Assert
        ruleInfoProvider.AreRulesBeingApplied = true;
        ruleInfoProvider.AreRulesBeingApplied.Should().BeTrue();

        ruleInfoProvider.AreRulesBeingApplied = false;
        ruleInfoProvider.AreRulesBeingApplied.Should().BeFalse();

        ruleInfoProvider.AreRulesBeingApplied = true;
        ruleInfoProvider.AreRulesBeingApplied.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenCalledMultipleTimes_ShouldCreateIndependentInstances()
    {
        // Act
        var provider1 = new RuleInfoProvider(
            updateIslandColorsMessageFactoryMock.Object,
            ruleMessageClearedMessageFactoryMock.Object);

        var provider2 = new RuleInfoProvider(
            updateIslandColorsMessageFactoryMock.Object,
            ruleMessageClearedMessageFactoryMock.Object);

        // Assert
        provider1.Should().NotBeSameAs(provider2);
        provider1.AreRulesBeingApplied = true;
        provider2.AreRulesBeingApplied.Should().BeFalse();
    }
}