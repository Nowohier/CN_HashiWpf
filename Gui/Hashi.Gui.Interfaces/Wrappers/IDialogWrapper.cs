using Hashi.Enums;

namespace Hashi.Gui.Interfaces.Wrappers;

/// <summary>
///     Interface for a dialog wrapper.
/// </summary>
public interface IDialogWrapper
{
    /// <summary>
    ///     Shows a dialog with the specified caption, message, button and image.
    /// </summary>
    /// <param name="caption">The caption.</param>
    /// <param name="message">The message.</param>
    /// <param name="button">The button.</param>
    /// <param name="image">The image.</param>
    /// <returns>a <see cref="DialogResult" />.</returns>
    public DialogResult Show(string caption, string message, DialogButton button = DialogButton.Ok,
        DialogImage image = DialogImage.None);
}