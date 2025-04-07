using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Views;

namespace Hashi.Gui.Wrappers;

/// <inheritdoc cref="IDialogWrapper" />
public class DialogWrapper : IDialogWrapper
{
    /// <inheritdoc />
    public DialogResult Show(string caption, string message, DialogButton button = DialogButton.Ok,
        DialogImage image = DialogImage.None)
    {
        return Dialog.Show(caption, message, button, image);
    }
}