using System.Windows;
using Hashi.Enums;
using Hashi.Gui.Interfaces.General;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Views;

public partial class Dialog : ICloseable, IDialogResult
{
    private Dialog(string caption, string message, DialogButton button = DialogButton.Ok,
        DialogImage image = DialogImage.None)
    {
        DataContext = new DialogViewModel(caption, message, button, image);
        InitializeComponent();
    }

    public new DialogResult DialogResult { get; set; }

    public static DialogResult Show(string caption, string message, DialogButton button = DialogButton.Ok,
        DialogImage image = DialogImage.None)
    {
        var window = new Dialog(caption, message, button, image)
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();
        var res = window.DialogResult;

        return res;
    }
}