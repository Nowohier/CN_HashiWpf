using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hashi.Enums;
using Hashi.Gui.Interfaces.General;

namespace Hashi.Gui.ViewModels;

/// <summary>
///     This view model handles all dialogs that are displayed from within the program.
/// </summary>
public class DialogViewModel : ObservableObject
{
    private string? caption, message;

    private bool isAsterisk,
        isError,
        isExclamation,
        isHand,
        isInformation,
        isNone,
        isQuestion,
        isStop,
        isWarning,
        isSuccess;

    private bool showOk, showCancel, showYesNo;

    /// <summary>
    ///     The viewmodel for the custom DialogView.
    /// </summary>
    /// <param name="caption">The dialog title</param>
    /// <param name="message">The dialog message</param>
    /// <param name="button">The buttons to display</param>
    /// <param name="image">The image to show</param>
    public DialogViewModel(string? caption, string? message, DialogButton button = DialogButton.Ok,
        DialogImage image = DialogImage.None)
    {
        OkCommand = new RelayCommand<object>(OkExecute);
        CancelCommand = new RelayCommand<object>(CancelExecute);
        YesCommand = new RelayCommand<object>(YesExecute);
        NoCommand = new RelayCommand<object>(NoExecute);

        Caption = caption;
        Message = message;
        SetImage(image);
        SetButtons(button);
    }

    /// <summary>
    ///     The title of the dialog.
    /// </summary>
    public string? Caption
    {
        get => caption;
        set => SetProperty(ref caption, value);
    }

    /// <summary>
    ///     The message of the dialog.
    /// </summary>
    public string? Message
    {
        get => message;
        set => SetProperty(ref message, value);
    }

    /// <summary>
    ///     The IsAsterisk property.
    /// </summary>
    public bool IsAsterisk
    {
        get => isAsterisk;
        set => SetProperty(ref isAsterisk, value);
    }

    /// <summary>
    ///     The IsSuccess property.
    /// </summary>
    public bool IsSuccess
    {
        get => isSuccess;
        set => SetProperty(ref isSuccess, value);
    }

    /// <summary>
    ///     The IsError property.
    /// </summary>
    public bool IsError
    {
        get => isError;
        set => SetProperty(ref isError, value);
    }

    /// <summary>
    ///     The IsExclamation property.
    /// </summary>
    public bool IsExclamation
    {
        get => isExclamation;
        set => SetProperty(ref isExclamation, value);
    }

    /// <summary>
    ///     The IsHand property.
    /// </summary>
    public bool IsHand
    {
        get => isHand;
        set => SetProperty(ref isHand, value);
    }

    /// <summary>
    ///     The IsNone property.
    /// </summary>
    public bool IsInformation
    {
        get => isInformation;
        set => SetProperty(ref isInformation, value);
    }

    /// <summary>
    ///     The IsNone property.
    /// </summary>
    public bool IsNone
    {
        get => isNone;
        set => SetProperty(ref isNone, value);
    }

    /// <summary>
    ///     The IsQuestion property.
    /// </summary>
    public bool IsQuestion
    {
        get => isQuestion;
        set => SetProperty(ref isQuestion, value);
    }

    /// <summary>
    ///     The IsStop property.
    /// </summary>
    public bool IsStop
    {
        get => isStop;
        set => SetProperty(ref isStop, value);
    }

    /// <summary>
    ///     The IsWarning property.
    /// </summary>
    public bool IsWarning
    {
        get => isWarning;
        set => SetProperty(ref isWarning, value);
    }

    /// <summary>
    ///     The ShowOk property.
    /// </summary>
    public bool ShowOk
    {
        get => showOk;
        set => SetProperty(ref showOk, value);
    }

    /// <summary>
    ///     The ShowCancel property.
    /// </summary>
    public bool ShowCancel
    {
        get => showCancel;
        set => SetProperty(ref showCancel, value);
    }

    /// <summary>
    ///     The ShowYesNo property.
    /// </summary>
    public bool ShowYesNo
    {
        get => showYesNo;
        set => SetProperty(ref showYesNo, value);
    }

    /// <summary>
    ///     The ok command.
    /// </summary>
    public ICommand OkCommand { get; set; }

    /// <summary>
    ///     The cancel command.
    /// </summary>
    public ICommand CancelCommand { get; set; }

    /// <summary>
    ///     The yes command.
    /// </summary>
    public ICommand YesCommand { get; set; }

    /// <summary>
    ///     The no command.
    /// </summary>
    public ICommand NoCommand { get; set; }

    private void OkExecute(object? obj)
    {
        if (obj is IDialogResult dia1) dia1.DialogResult = DialogResult.Ok;
        if (obj is ICloseable dia2) dia2.Close();
    }

    private void CancelExecute(object? obj)
    {
        if (obj is IDialogResult dia1) dia1.DialogResult = DialogResult.Cancel;
        if (obj is ICloseable dia2) dia2.Close();
    }

    private void YesExecute(object? obj)
    {
        if (obj is IDialogResult dia1) dia1.DialogResult = DialogResult.Yes;
        if (obj is ICloseable dia2) dia2.Close();
    }

    private void NoExecute(object? obj)
    {
        if (obj is IDialogResult dia1) dia1.DialogResult = DialogResult.No;
        if (obj is ICloseable dia2) dia2.Close();
    }

    private void SetButtons(DialogButton button)
    {
        (ShowOk, ShowCancel, ShowYesNo) = button switch
        {
            DialogButton.Ok => (true, false, false),
            DialogButton.OkCancel => (true, true, false),
            DialogButton.YesNo => (false, false, true),
            DialogButton.YesNoCancel => (false, true, true),
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
        };
    }

    private void SetImage(DialogImage image)
    {
        (IsAsterisk, IsError, IsExclamation, IsHand, IsInformation, IsNone, IsQuestion, IsStop, IsWarning, IsSuccess) =
            image switch
            {
                DialogImage.Asterisk => (true, false, false, false, false, false, false, false, false, false),
                DialogImage.Error => (false, true, false, false, false, false, false, false, false, false),
                DialogImage.Exclamation => (false, false, true, false, false, false, false, false, false, false),
                DialogImage.Hand => (false, false, false, true, false, false, false, false, false, false),
                DialogImage.Information => (false, false, false, false, true, false, false, false, false, false),
                DialogImage.None => (false, false, false, false, false, true, false, false, false, false),
                DialogImage.Question => (false, false, false, false, false, false, true, false, false, false),
                DialogImage.Stop => (false, false, false, false, false, false, false, true, false, false),
                DialogImage.Warning => (false, false, false, false, false, false, false, false, true, false),
                DialogImage.Success => (false, false, false, false, false, false, false, false, false, true),
                _ => throw new ArgumentOutOfRangeException(nameof(image), image, null)
            };
    }
}