using CommunityToolkit.Mvvm.Messaging;
using FluentAssertions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Providers;
using Moq;

namespace Hashi.Gui.Test.Providers;

[TestFixture]
public class RuleInfoProviderTests : IRecipient<IUpdateAllIslandColorsMessage>, IRecipient<IRuleMessageClearedMessage>
{
    private Mock<Func<bool?, IUpdateAllIslandColorsMessage>> updateColorsFactoryMock;
    private Mock<Func<bool?, IRuleMessageClearedMessage>> ruleMessageClearedFactoryMock;
    private RuleInfoProvider ruleInfoProvider;

    [SetUp]
    public void SetUp()
    {
        updateColorsFactoryMock = new Mock<Func<bool?, IUpdateAllIslandColorsMessage>>(MockBehavior.Strict);
        ruleMessageClearedFactoryMock = new Mock<Func<bool?, IRuleMessageClearedMessage>>(MockBehavior.Strict);

        WeakReferenceMessenger.Default.Reset();
        WeakReferenceMessenger.Default.RegisterAll(this);

        ruleInfoProvider = new RuleInfoProvider(
            updateColorsFactoryMock.Object,
            ruleMessageClearedFactoryMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    void IRecipient<IUpdateAllIslandColorsMessage>.Receive(IUpdateAllIslandColorsMessage message) { }
    void IRecipient<IRuleMessageClearedMessage>.Receive(IRuleMessageClearedMessage message) { }

    #region RuleMessage Tests

    [Test]
    public void RuleMessage_WhenSetToNonEmpty_ShouldNotSendMessages()
    {
        // Act
        ruleInfoProvider.RuleMessage = "Some rule hint";

        // Assert
        ruleInfoProvider.RuleMessage.Should().Be("Some rule hint");
        updateColorsFactoryMock.Verify(x => x(It.IsAny<bool?>()), Times.Never);
        ruleMessageClearedFactoryMock.Verify(x => x(It.IsAny<bool?>()), Times.Never);
    }

    [Test]
    public void RuleMessage_WhenClearedToEmpty_ShouldSendBothMessages()
    {
        // Arrange
        var updateMsgMock = new Mock<IUpdateAllIslandColorsMessage>(MockBehavior.Strict);
        var clearedMsgMock = new Mock<IRuleMessageClearedMessage>(MockBehavior.Strict);

        updateColorsFactoryMock.Setup(x => x(null)).Returns(updateMsgMock.Object);
        ruleMessageClearedFactoryMock.Setup(x => x(null)).Returns(clearedMsgMock.Object);

        ruleInfoProvider.RuleMessage = "Some rule hint";

        // Act
        ruleInfoProvider.RuleMessage = string.Empty;

        // Assert
        ruleInfoProvider.RuleMessage.Should().BeEmpty();
        updateColorsFactoryMock.Verify(x => x(null), Times.Once);
        ruleMessageClearedFactoryMock.Verify(x => x(null), Times.Once);
    }

    [Test]
    public void RuleMessage_WhenSetToSameValue_ShouldNotSendMessages()
    {
        // Arrange - default is already string.Empty

        // Act
        ruleInfoProvider.RuleMessage = string.Empty;

        // Assert
        updateColorsFactoryMock.Verify(x => x(It.IsAny<bool?>()), Times.Never);
        ruleMessageClearedFactoryMock.Verify(x => x(It.IsAny<bool?>()), Times.Never);
    }

    #endregion

    #region AreRulesBeingApplied Tests

    [Test]
    public void AreRulesBeingApplied_WhenSet_ShouldUpdateValue()
    {
        // Act
        ruleInfoProvider.AreRulesBeingApplied = true;

        // Assert
        ruleInfoProvider.AreRulesBeingApplied.Should().BeTrue();
    }

    [Test]
    public void AreRulesBeingApplied_WhenDefault_ShouldBeFalse()
    {
        // Assert
        ruleInfoProvider.AreRulesBeingApplied.Should().BeFalse();
    }

    #endregion
}
