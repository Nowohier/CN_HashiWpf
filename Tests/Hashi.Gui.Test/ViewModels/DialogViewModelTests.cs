using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.General;
using Hashi.Gui.ViewModels;
using Moq;

namespace Hashi.Gui.Test.ViewModels;

[TestFixture]
public class DialogViewModelTests
{
    private DialogViewModel sut;

    [SetUp]
    public void SetUp()
    {
        sut = new DialogViewModel("Test Caption", "Test Message");
    }

    [Test]
    public void Constructor_WhenCalledWithCaptionAndMessage_ShouldSetProperties()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test Caption", "Test Message");

        // Assert
        result.Caption.Should().Be("Test Caption");
        result.Message.Should().Be("Test Message");
        result.ShowOk.Should().BeTrue();
        result.ShowCancel.Should().BeFalse();
        result.ShowYesNo.Should().BeFalse();
        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenCalledWithDialogButtonOk_ShouldShowOnlyOkButton()
    {
        // Arrange & Act
        var result = new DialogViewModel("Caption", "Message", DialogButton.Ok);

        // Assert
        result.ShowOk.Should().BeTrue();
        result.ShowCancel.Should().BeFalse();
        result.ShowYesNo.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithDialogButtonOkCancel_ShouldShowOkAndCancelButtons()
    {
        // Arrange & Act
        var result = new DialogViewModel("Caption", "Message", DialogButton.OkCancel);

        // Assert
        result.ShowOk.Should().BeTrue();
        result.ShowCancel.Should().BeTrue();
        result.ShowYesNo.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithDialogButtonYesNo_ShouldShowYesNoButtons()
    {
        // Arrange & Act
        var result = new DialogViewModel("Caption", "Message", DialogButton.YesNo);

        // Assert
        result.ShowOk.Should().BeFalse();
        result.ShowCancel.Should().BeFalse();
        result.ShowYesNo.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenCalledWithDialogButtonYesNoCancel_ShouldShowYesNoAndCancelButtons()
    {
        // Arrange & Act
        var result = new DialogViewModel("Caption", "Message", DialogButton.YesNoCancel);

        // Assert
        result.ShowOk.Should().BeFalse();
        result.ShowCancel.Should().BeTrue();
        result.ShowYesNo.Should().BeTrue();
    }

    [TestCase(DialogImage.Asterisk, nameof(DialogViewModel.IsAsterisk))]
    [TestCase(DialogImage.Error, nameof(DialogViewModel.IsError))]
    [TestCase(DialogImage.Exclamation, nameof(DialogViewModel.IsExclamation))]
    [TestCase(DialogImage.Hand, nameof(DialogViewModel.IsHand))]
    [TestCase(DialogImage.Information, nameof(DialogViewModel.IsInformation))]
    [TestCase(DialogImage.None, nameof(DialogViewModel.IsNone))]
    [TestCase(DialogImage.Question, nameof(DialogViewModel.IsQuestion))]
    [TestCase(DialogImage.Stop, nameof(DialogViewModel.IsStop))]
    [TestCase(DialogImage.Warning, nameof(DialogViewModel.IsWarning))]
    [TestCase(DialogImage.Success, nameof(DialogViewModel.IsSuccess))]
    public void Constructor_WhenCalledWithSpecificDialogImage_ShouldSetCorrectImageProperty(DialogImage image, string expectedPropertyName)
    {
        // Arrange & Act
        var result = new DialogViewModel("Caption", "Message", DialogButton.Ok, image);

        // Assert
        var property = typeof(DialogViewModel).GetProperty(expectedPropertyName);
        property.Should().NotBeNull();
        ((bool)property!.GetValue(result)!).Should().BeTrue();
        
        // Verify only this property is true
        var allImageProperties = typeof(DialogViewModel).GetProperties()
            .Where(p => p.Name.StartsWith("Is") && p.PropertyType == typeof(bool))
            .Where(p => p.Name != expectedPropertyName);
        
        foreach (var prop in allImageProperties)
        {
            ((bool)prop.GetValue(result)!).Should().BeFalse($"{prop.Name} should be false when {expectedPropertyName} is true");
        }
    }

    [Test]
    public void Caption_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newCaption = "New Caption";

        // Act
        sut.Caption = newCaption;

        // Assert
        sut.Caption.Should().Be(newCaption);
    }

    [Test]
    public void Message_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newMessage = "New Message";

        // Act
        sut.Message = newMessage;

        // Assert
        sut.Message.Should().Be(newMessage);
    }

    [Test]
    public void OkCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToOk()
    {
        // Arrange
        var mockDialogResult = new Mock<IDialogResult>();
        var mockObject = Mock.Of<IDialogResult>(x => x == mockDialogResult.Object);

        // Act
        sut.OkCommand.Execute(mockObject);

        // Assert
        mockDialogResult.VerifySet(x => x.DialogResult = DialogResult.Ok, Times.Once);
    }

    [Test]
    public void OkCommand_WhenExecutedWithCloseable_ShouldCallClose()
    {
        // Arrange
        var mockCloseable = new Mock<ICloseable>();

        // Act
        sut.OkCommand.Execute(mockCloseable.Object);

        // Assert
        mockCloseable.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void CancelCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToCancel()
    {
        // Arrange
        var mockDialogResult = new Mock<IDialogResult>();
        var mockObject = Mock.Of<IDialogResult>(x => x == mockDialogResult.Object);

        // Act
        sut.CancelCommand.Execute(mockObject);

        // Assert
        mockDialogResult.VerifySet(x => x.DialogResult = DialogResult.Cancel, Times.Once);
    }

    [Test]
    public void CancelCommand_WhenExecutedWithCloseable_ShouldCallClose()
    {
        // Arrange
        var mockCloseable = new Mock<ICloseable>();

        // Act
        sut.CancelCommand.Execute(mockCloseable.Object);

        // Assert
        mockCloseable.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void YesCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToYes()
    {
        // Arrange
        var mockDialogResult = new Mock<IDialogResult>();
        var mockObject = Mock.Of<IDialogResult>(x => x == mockDialogResult.Object);

        // Act
        sut.YesCommand.Execute(mockObject);

        // Assert
        mockDialogResult.VerifySet(x => x.DialogResult = DialogResult.Yes, Times.Once);
    }

    [Test]
    public void YesCommand_WhenExecutedWithCloseable_ShouldCallClose()
    {
        // Arrange
        var mockCloseable = new Mock<ICloseable>();

        // Act
        sut.YesCommand.Execute(mockCloseable.Object);

        // Assert
        mockCloseable.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void NoCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToNo()
    {
        // Arrange
        var mockDialogResult = new Mock<IDialogResult>();
        var mockObject = Mock.Of<IDialogResult>(x => x == mockDialogResult.Object);

        // Act
        sut.NoCommand.Execute(mockObject);

        // Assert
        mockDialogResult.VerifySet(x => x.DialogResult = DialogResult.No, Times.Once);
    }

    [Test]
    public void NoCommand_WhenExecutedWithCloseable_ShouldCallClose()
    {
        // Arrange
        var mockCloseable = new Mock<ICloseable>();

        // Act
        sut.NoCommand.Execute(mockCloseable.Object);

        // Assert
        mockCloseable.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void Commands_WhenExecutedWithObjectImplementingBothInterfaces_ShouldCallBothMethods()
    {
        // Arrange
        var mockObject = new Mock<IDialogResultAndCloseable>();

        // Act
        sut.OkCommand.Execute(mockObject.Object);

        // Assert
        mockObject.VerifySet(x => x.DialogResult = DialogResult.Ok, Times.Once);
        mockObject.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void Commands_WhenExecutedWithNull_ShouldNotThrowException()
    {
        // Arrange & Act & Assert
        sut.Invoking(x => x.OkCommand.Execute(null)).Should().NotThrow();
        sut.Invoking(x => x.CancelCommand.Execute(null)).Should().NotThrow();
        sut.Invoking(x => x.YesCommand.Execute(null)).Should().NotThrow();
        sut.Invoking(x => x.NoCommand.Execute(null)).Should().NotThrow();
    }

    [Test]
    public void Commands_WhenExecutedWithIncompatibleObject_ShouldNotThrowException()
    {
        // Arrange
        var incompatibleObject = new object();

        // Act & Assert
        sut.Invoking(x => x.OkCommand.Execute(incompatibleObject)).Should().NotThrow();
        sut.Invoking(x => x.CancelCommand.Execute(incompatibleObject)).Should().NotThrow();
        sut.Invoking(x => x.YesCommand.Execute(incompatibleObject)).Should().NotThrow();
        sut.Invoking(x => x.NoCommand.Execute(incompatibleObject)).Should().NotThrow();
    }

    // Helper interface for testing objects that implement both interfaces
    public interface IDialogResultAndCloseable : IDialogResult, ICloseable
    {
    }
}