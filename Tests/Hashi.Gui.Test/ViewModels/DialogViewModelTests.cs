using FluentAssertions;
using Hashi.Enums;
using Hashi.Gui.ViewModels;
using System.ComponentModel;
using System.Windows.Input;

namespace Hashi.Gui.Test.ViewModels;

/// <summary>
/// Unit tests for DialogViewModel class.
/// </summary>
public class DialogViewModelTests
{
    [Test]
    public void Constructor_WhenValidParameters_ShouldSetCaptionAndMessage()
    {
        // Arrange
        const string caption = "Test Caption";
        const string message = "Test Message";

        // Act
        var sut = new DialogViewModel(caption, message);

        // Assert
        sut.Caption.Should().Be(caption);
        sut.Message.Should().Be(message);
    }

    [Test]
    public void Constructor_WhenNullParameters_ShouldSetPropertiesToNull()
    {
        // Arrange & Act
        var sut = new DialogViewModel(null, null);

        // Assert
        sut.Caption.Should().BeNull();
        sut.Message.Should().BeNull();
    }

    [Test]
    public void Constructor_ShouldImplementINotifyPropertyChanged()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test");

        // Assert
        sut.Should().BeAssignableTo<INotifyPropertyChanged>();
    }

    [Test]
    public void Constructor_WhenDialogButtonOk_ShouldSetShowOkToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.Ok);

        // Assert
        sut.ShowOk.Should().BeTrue();
        sut.ShowCancel.Should().BeFalse();
        sut.ShowYesNo.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenDialogButtonOkCancel_ShouldSetBothOkAndCancelToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.OkCancel);

        // Assert
        sut.ShowOk.Should().BeTrue();
        sut.ShowCancel.Should().BeTrue();
        sut.ShowYesNo.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenDialogButtonYesNo_ShouldSetShowYesNoToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.YesNo);

        // Assert
        sut.ShowYesNo.Should().BeTrue();
        sut.ShowOk.Should().BeFalse();
        sut.ShowCancel.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenDialogImageNone_ShouldSetIsNoneToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.None);

        // Assert
        sut.IsNone.Should().BeTrue();
        sut.IsError.Should().BeFalse();
        sut.IsInformation.Should().BeFalse();
        sut.IsWarning.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenDialogImageError_ShouldSetIsErrorToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Error);

        // Assert
        sut.IsError.Should().BeTrue();
        sut.IsNone.Should().BeFalse();
        sut.IsInformation.Should().BeFalse();
        sut.IsWarning.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenDialogImageInformation_ShouldSetIsInformationToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Information);

        // Assert
        sut.IsInformation.Should().BeTrue();
        sut.IsError.Should().BeFalse();
        sut.IsNone.Should().BeFalse();
        sut.IsWarning.Should().BeFalse();
    }

    [Test]
    public void Constructor_WhenDialogImageWarning_ShouldSetIsWarningToTrue()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test", DialogButton.Ok, DialogImage.Warning);

        // Assert
        sut.IsWarning.Should().BeTrue();
        sut.IsError.Should().BeFalse();
        sut.IsNone.Should().BeFalse();
        sut.IsInformation.Should().BeFalse();
    }

    [Test]
    public void Constructor_ShouldInitializeCommands()
    {
        // Arrange & Act
        var sut = new DialogViewModel("Test", "Test");

        // Assert
        sut.OkCommand.Should().NotBeNull();
        sut.OkCommand.Should().BeAssignableTo<ICommand>();
        sut.CancelCommand.Should().NotBeNull();
        sut.CancelCommand.Should().BeAssignableTo<ICommand>();
        sut.YesCommand.Should().NotBeNull();
        sut.YesCommand.Should().BeAssignableTo<ICommand>();
        sut.NoCommand.Should().NotBeNull();
        sut.NoCommand.Should().BeAssignableTo<ICommand>();
    }

    [Test]
    public void Caption_WhenSet_ShouldUpdatePropertyAndRaisePropertyChanged()
    {
        // Arrange
        var sut = new DialogViewModel("Initial", "Test");
        const string newCaption = "New Caption";
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DialogViewModel.Caption))
                propertyChangedRaised = true;
        };

        // Act
        sut.Caption = newCaption;

        // Assert
        sut.Caption.Should().Be(newCaption);
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void Message_WhenSet_ShouldUpdatePropertyAndRaisePropertyChanged()
    {
        // Arrange
        var sut = new DialogViewModel("Test", "Initial");
        const string newMessage = "New Message";
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DialogViewModel.Message))
                propertyChangedRaised = true;
        };

        // Act
        sut.Message = newMessage;

        // Assert
        sut.Message.Should().Be(newMessage);
        propertyChangedRaised.Should().BeTrue();
    }

    [Test]
    public void IsError_WhenSet_ShouldUpdatePropertyAndRaisePropertyChanged()
    {
        // Arrange
        var sut = new DialogViewModel("Test", "Test");
        var propertyChangedRaised = false;

        sut.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(DialogViewModel.IsError))
                propertyChangedRaised = true;
        };

        // Act
        sut.IsError = true;

        // Assert
        sut.IsError.Should().BeTrue();
        propertyChangedRaised.Should().BeTrue();
    }
}