using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.General;
using Hashi.Gui.ViewModels;
using Moq;
using System.Windows.Input;

namespace Hashi.Gui.Test.ViewModels;

[TestFixture]
public class DialogViewModelTests
{
    private DialogViewModel dialogViewModel;
    private Mock<IDialogResult> dialogResultMock;
    private Mock<ICloseable> closeableMock;

    [SetUp]
    public void SetUp()
    {
        dialogResultMock = new Mock<IDialogResult>(MockBehavior.Strict);
        closeableMock = new Mock<ICloseable>(MockBehavior.Strict);

        dialogViewModel = new DialogViewModel("Test Caption", "Test Message", DialogButton.Ok, DialogImage.None);
    }

    [Test]
    public void Constructor_WhenValidParameters_ShouldInitializeProperties()
    {
        // Arrange
        var caption = "Test Caption";
        var message = "Test Message";
        var button = DialogButton.Ok;
        var image = DialogImage.Information;

        // Act
        var result = new DialogViewModel(caption, message, button, image);

        // Assert
        result.Caption.Should().Be(caption);
        result.Message.Should().Be(message);
        result.ShowOk.Should().BeTrue();
        result.ShowCancel.Should().BeFalse();
        result.ShowYesNo.Should().BeFalse();
        result.IsInformation.Should().BeTrue();
        result.IsNone.Should().BeFalse();
        result.OkCommand.Should().NotBeNull();
        result.CancelCommand.Should().NotBeNull();
        result.YesCommand.Should().NotBeNull();
        result.NoCommand.Should().NotBeNull();
    }

    [Test]
    public void Constructor_WhenNullCaption_ShouldAcceptNullCaption()
    {
        // Arrange
        string? caption = null;
        var message = "Test Message";

        // Act
        var result = new DialogViewModel(caption, message);

        // Assert
        result.Caption.Should().BeNull();
        result.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WhenNullMessage_ShouldAcceptNullMessage()
    {
        // Arrange
        var caption = "Test Caption";
        string? message = null;

        // Act
        var result = new DialogViewModel(caption, message);

        // Assert
        result.Caption.Should().Be(caption);
        result.Message.Should().BeNull();
    }

    [Test]
    public void Caption_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newCaption = "New Caption";

        // Act
        dialogViewModel.Caption = newCaption;

        // Assert
        dialogViewModel.Caption.Should().Be(newCaption);
    }

    [Test]
    public void Message_WhenSet_ShouldUpdateProperty()
    {
        // Arrange
        var newMessage = "New Message";

        // Act
        dialogViewModel.Message = newMessage;

        // Assert
        dialogViewModel.Message.Should().Be(newMessage);
    }

    [Test]
    [TestCase(DialogButton.Ok, true, false, false)]
    [TestCase(DialogButton.OkCancel, true, true, false)]
    [TestCase(DialogButton.YesNo, false, false, true)]
    [TestCase(DialogButton.YesNoCancel, false, true, true)]
    public void Constructor_WhenDifferentButtons_ShouldSetCorrectButtonVisibility(
        DialogButton button, bool expectedShowOk, bool expectedShowCancel, bool expectedShowYesNo)
    {
        // Act
        var result = new DialogViewModel("Caption", "Message", button);

        // Assert
        result.ShowOk.Should().Be(expectedShowOk);
        result.ShowCancel.Should().Be(expectedShowCancel);
        result.ShowYesNo.Should().Be(expectedShowYesNo);
    }

    [Test]
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
    public void Constructor_WhenDifferentImages_ShouldSetCorrectImageProperty(DialogImage image, string expectedPropertyName)
    {
        // Act
        var result = new DialogViewModel("Caption", "Message", DialogButton.Ok, image);

        // Assert
        var property = result.GetType().GetProperty(expectedPropertyName);
        property.Should().NotBeNull();
        var value = (bool?)property?.GetValue(result);
        value.Should().BeTrue();
    }

    [Test]
    public void OkCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToOk()
    {
        // Arrange
        var dialogMock = new Mock<IDialogResult>(MockBehavior.Strict);
        dialogMock.SetupSet(x => x.DialogResult = DialogResult.Ok);

        // Act
        dialogViewModel.OkCommand.Execute(dialogMock.Object);

        // Assert
        dialogMock.VerifySet(x => x.DialogResult = DialogResult.Ok, Times.Once);
    }

    [Test]
    public void OkCommand_WhenExecutedWithCloseable_ShouldCallClose()
    {
        // Arrange
        var closeableMock = new Mock<ICloseable>(MockBehavior.Strict);
        closeableMock.Setup(x => x.Close());

        // Act
        dialogViewModel.OkCommand.Execute(closeableMock.Object);

        // Assert
        closeableMock.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void CancelCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToCancel()
    {
        // Arrange
        var dialogMock = new Mock<IDialogResult>(MockBehavior.Strict);
        dialogMock.SetupSet(x => x.DialogResult = DialogResult.Cancel);

        // Act
        dialogViewModel.CancelCommand.Execute(dialogMock.Object);

        // Assert
        dialogMock.VerifySet(x => x.DialogResult = DialogResult.Cancel, Times.Once);
    }

    [Test]
    public void YesCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToYes()
    {
        // Arrange
        var dialogMock = new Mock<IDialogResult>(MockBehavior.Strict);
        dialogMock.SetupSet(x => x.DialogResult = DialogResult.Yes);

        // Act
        dialogViewModel.YesCommand.Execute(dialogMock.Object);

        // Assert
        dialogMock.VerifySet(x => x.DialogResult = DialogResult.Yes, Times.Once);
    }

    [Test]
    public void NoCommand_WhenExecutedWithDialogResult_ShouldSetDialogResultToNo()
    {
        // Arrange
        var dialogMock = new Mock<IDialogResult>(MockBehavior.Strict);
        dialogMock.SetupSet(x => x.DialogResult = DialogResult.No);

        // Act
        dialogViewModel.NoCommand.Execute(dialogMock.Object);

        // Assert
        dialogMock.VerifySet(x => x.DialogResult = DialogResult.No, Times.Once);
    }

    [Test]
    public void AllImageProperties_WhenSet_ShouldUpdateProperty()
    {
        // Act & Assert
        dialogViewModel.IsAsterisk = true;
        dialogViewModel.IsAsterisk.Should().BeTrue();

        dialogViewModel.IsError = true;
        dialogViewModel.IsError.Should().BeTrue();

        dialogViewModel.IsExclamation = true;
        dialogViewModel.IsExclamation.Should().BeTrue();

        dialogViewModel.IsHand = true;
        dialogViewModel.IsHand.Should().BeTrue();

        dialogViewModel.IsInformation = true;
        dialogViewModel.IsInformation.Should().BeTrue();

        dialogViewModel.IsNone = true;
        dialogViewModel.IsNone.Should().BeTrue();

        dialogViewModel.IsQuestion = true;
        dialogViewModel.IsQuestion.Should().BeTrue();

        dialogViewModel.IsStop = true;
        dialogViewModel.IsStop.Should().BeTrue();

        dialogViewModel.IsWarning = true;
        dialogViewModel.IsWarning.Should().BeTrue();

        dialogViewModel.IsSuccess = true;
        dialogViewModel.IsSuccess.Should().BeTrue();
    }

    [Test]
    public void AllButtonProperties_WhenSet_ShouldUpdateProperty()
    {
        // Act & Assert
        dialogViewModel.ShowOk = false;
        dialogViewModel.ShowOk.Should().BeFalse();

        dialogViewModel.ShowCancel = true;
        dialogViewModel.ShowCancel.Should().BeTrue();

        dialogViewModel.ShowYesNo = true;
        dialogViewModel.ShowYesNo.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenInvalidDialogButton_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        var act = () => new DialogViewModel("Caption", "Message", (DialogButton)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Constructor_WhenInvalidDialogImage_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        var act = () => new DialogViewModel("Caption", "Message", DialogButton.Ok, (DialogImage)999);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Commands_WhenExecutedWithNull_ShouldNotThrow()
    {
        // Act & Assert
        var act1 = () => dialogViewModel.OkCommand.Execute(null);
        act1.Should().NotThrow();

        var act2 = () => dialogViewModel.CancelCommand.Execute(null);
        act2.Should().NotThrow();

        var act3 = () => dialogViewModel.YesCommand.Execute(null);
        act3.Should().NotThrow();

        var act4 = () => dialogViewModel.NoCommand.Execute(null);
        act4.Should().NotThrow();
    }

    [Test]
    public void Commands_WhenExecutedWithObjectThatImplementsBothInterfaces_ShouldSetDialogResultAndClose()
    {
        // Arrange
        var combinedMock = new Mock<IDialogResult>(MockBehavior.Strict);
        combinedMock.As<ICloseable>();
        combinedMock.SetupSet(x => x.DialogResult = DialogResult.Ok);
        combinedMock.As<ICloseable>().Setup(x => x.Close());

        // Act
        dialogViewModel.OkCommand.Execute(combinedMock.Object);

        // Assert
        combinedMock.VerifySet(x => x.DialogResult = DialogResult.Ok, Times.Once);
        combinedMock.As<ICloseable>().Verify(x => x.Close(), Times.Once);
    }
}