using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.Interfaces.General;
using Hashi.Gui.ViewModels;
using Moq;

namespace Hashi.Gui.Test.ViewModels;

[TestFixture]
public class DialogViewModelTests
{
    private DialogViewModel dialogViewModel;

    [SetUp]
    public void SetUp()
    {
        dialogViewModel = new DialogViewModel("Test Caption", "Test Message");
    }

    #region Constructor Tests

    [Test]
    public void Constructor_WhenCalledWithDefaults_ShouldInitializeProperties()
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
        result.OkCommand.Should().NotBeNull();
        result.CancelCommand.Should().NotBeNull();
        result.YesCommand.Should().NotBeNull();
        result.NoCommand.Should().NotBeNull();
    }

    [Test]
    public void Constructor_WhenCalledWithOkCancelButton_ShouldSetCorrectButtonVisibility()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.OkCancel);

        // Assert
        result.ShowOk.Should().BeTrue();
        result.ShowCancel.Should().BeTrue();
        result.ShowYesNo.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithYesNoButton_ShouldSetCorrectButtonVisibility()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.YesNo);

        // Assert
        result.ShowOk.Should().BeFalse();
        result.ShowCancel.Should().BeFalse();
        result.ShowYesNo.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenCalledWithYesNoCancelButton_ShouldSetCorrectButtonVisibility()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.YesNoCancel);

        // Assert
        result.ShowOk.Should().BeFalse();
        result.ShowCancel.Should().BeTrue();
        result.ShowYesNo.Should().BeTrue();
    }

    [Test]
    public void Constructor_WhenCalledWithInvalidButton_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act & Assert
        var action = () => new DialogViewModel("Test", "Test", (DialogButton)999);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Image Property Tests

    [Test]
    public void Constructor_WhenCalledWithAsteriskImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Asterisk);

        // Assert
        result.IsAsterisk.Should().BeTrue();
        result.IsError.Should().BeFalse();
        result.IsExclamation.Should().BeFalse();
        result.IsHand.Should().BeFalse();
        result.IsInformation.Should().BeFalse();
        result.IsNone.Should().BeFalse();
        result.IsQuestion.Should().BeFalse();
        result.IsStop.Should().BeFalse();
        result.IsWarning.Should().BeFalse();
        result.IsSuccess.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithErrorImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Error);

        // Assert
        result.IsError.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithExclamationImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Exclamation);

        // Assert
        result.IsExclamation.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithHandImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Hand);

        // Assert
        result.IsHand.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithInformationImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Information);

        // Assert
        result.IsInformation.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithQuestionImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Question);

        // Assert
        result.IsQuestion.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithStopImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Stop);

        // Assert
        result.IsStop.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithWarningImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Warning);

        // Assert
        result.IsWarning.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithSuccessImage_ShouldSetCorrectImageProperty()
    {
        // Arrange & Act
        var result = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Success);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsAsterisk.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenCalledWithInvalidImage_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange & Act & Assert
        var action = () => new DialogViewModel("Test", "Test", DialogButton.Ok, (DialogImage)999);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Property Tests

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
    public void IsAsterisk_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsAsterisk = true;

        // Assert
        dialogViewModel.IsAsterisk.Should().BeTrue();
    }

    [Test]
    public void IsSuccess_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsSuccess = true;

        // Assert
        dialogViewModel.IsSuccess.Should().BeTrue();
    }

    [Test]
    public void IsError_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsError = true;

        // Assert
        dialogViewModel.IsError.Should().BeTrue();
    }

    [Test]
    public void IsExclamation_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsExclamation = true;

        // Assert
        dialogViewModel.IsExclamation.Should().BeTrue();
    }

    [Test]
    public void IsHand_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsHand = true;

        // Assert
        dialogViewModel.IsHand.Should().BeTrue();
    }

    [Test]
    public void IsInformation_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsInformation = true;

        // Assert
        dialogViewModel.IsInformation.Should().BeTrue();
    }

    [Test]
    public void IsNone_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsNone = false;

        // Assert
        dialogViewModel.IsNone.Should().BeFalse();
    }

    [Test]
    public void IsQuestion_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsQuestion = true;

        // Assert
        dialogViewModel.IsQuestion.Should().BeTrue();
    }

    [Test]
    public void IsStop_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsStop = true;

        // Assert
        dialogViewModel.IsStop.Should().BeTrue();
    }

    [Test]
    public void IsWarning_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.IsWarning = true;

        // Assert
        dialogViewModel.IsWarning.Should().BeTrue();
    }

    [Test]
    public void ShowOk_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.ShowOk = false;

        // Assert
        dialogViewModel.ShowOk.Should().BeFalse();
    }

    [Test]
    public void ShowCancel_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.ShowCancel = true;

        // Assert
        dialogViewModel.ShowCancel.Should().BeTrue();
    }

    [Test]
    public void ShowYesNo_WhenSet_ShouldUpdateProperty()
    {
        // Act
        dialogViewModel.ShowYesNo = true;

        // Assert
        dialogViewModel.ShowYesNo.Should().BeTrue();
    }

    #endregion

    #region Command Tests

    [Test]
    public void OkCommand_WhenExecutedWithDialogResultAndCloseable_ShouldSetResultAndClose()
    {
        // Arrange
        var mockObject = new Mock<ITestDialogAndCloseable>(MockBehavior.Strict);
        mockObject.SetupSet(x => x.DialogResult = DialogResult.Ok);
        mockObject.Setup(x => x.Close());

        // Act
        dialogViewModel.OkCommand.Execute(mockObject.Object);

        // Assert
        mockObject.VerifySet(x => x.DialogResult = DialogResult.Ok, Times.Once);
        mockObject.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void OkCommand_WhenExecutedWithNull_ShouldNotThrow()
    {
        // Act & Assert
        var action = () => dialogViewModel.OkCommand.Execute(null);
        action.Should().NotThrow();
    }

    [Test]
    public void CancelCommand_WhenExecutedWithDialogResultAndCloseable_ShouldSetResultAndClose()
    {
        // Arrange
        var mockObject = new Mock<ITestDialogAndCloseable>(MockBehavior.Strict);
        mockObject.SetupSet(x => x.DialogResult = DialogResult.Cancel);
        mockObject.Setup(x => x.Close());

        // Act
        dialogViewModel.CancelCommand.Execute(mockObject.Object);

        // Assert
        mockObject.VerifySet(x => x.DialogResult = DialogResult.Cancel, Times.Once);
        mockObject.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void YesCommand_WhenExecutedWithDialogResultAndCloseable_ShouldSetResultAndClose()
    {
        // Arrange
        var mockObject = new Mock<ITestDialogAndCloseable>(MockBehavior.Strict);
        mockObject.SetupSet(x => x.DialogResult = DialogResult.Yes);
        mockObject.Setup(x => x.Close());

        // Act
        dialogViewModel.YesCommand.Execute(mockObject.Object);

        // Assert
        mockObject.VerifySet(x => x.DialogResult = DialogResult.Yes, Times.Once);
        mockObject.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void NoCommand_WhenExecutedWithDialogResultAndCloseable_ShouldSetResultAndClose()
    {
        // Arrange
        var mockObject = new Mock<ITestDialogAndCloseable>(MockBehavior.Strict);
        mockObject.SetupSet(x => x.DialogResult = DialogResult.No);
        mockObject.Setup(x => x.Close());

        // Act
        dialogViewModel.NoCommand.Execute(mockObject.Object);

        // Assert
        mockObject.VerifySet(x => x.DialogResult = DialogResult.No, Times.Once);
        mockObject.Verify(x => x.Close(), Times.Once);
    }

    [Test]
    public void Commands_WhenExecutedWithOnlyDialogResult_ShouldOnlySetResult()
    {
        // Arrange
        var mockDialogResult = new Mock<IDialogResult>(MockBehavior.Strict);
        mockDialogResult.SetupSet(x => x.DialogResult = It.IsAny<DialogResult>());

        // Act & Assert
        var actions = new Action[]
        {
            () => dialogViewModel.OkCommand.Execute(mockDialogResult.Object),
            () => dialogViewModel.CancelCommand.Execute(mockDialogResult.Object),
            () => dialogViewModel.YesCommand.Execute(mockDialogResult.Object),
            () => dialogViewModel.NoCommand.Execute(mockDialogResult.Object)
        };

        foreach (var action in actions)
        {
            action.Should().NotThrow();
        }
    }

    [Test]
    public void Commands_WhenExecutedWithOnlyCloseable_ShouldOnlyClose()
    {
        // Arrange
        var mockCloseable = new Mock<ICloseable>(MockBehavior.Strict);
        mockCloseable.Setup(x => x.Close());

        // Act & Assert
        var actions = new Action[]
        {
            () => dialogViewModel.OkCommand.Execute(mockCloseable.Object),
            () => dialogViewModel.CancelCommand.Execute(mockCloseable.Object),
            () => dialogViewModel.YesCommand.Execute(mockCloseable.Object),
            () => dialogViewModel.NoCommand.Execute(mockCloseable.Object)
        };

        foreach (var action in actions)
        {
            action.Should().NotThrow();
        }

        mockCloseable.Verify(x => x.Close(), Times.Exactly(4));
    }

    #endregion
}

// Helper interface for testing both IDialogResult and ICloseable
public interface ITestDialogAndCloseable : IDialogResult, ICloseable
{
}