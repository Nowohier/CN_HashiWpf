using Hashi.Enums;

namespace Hashi.Gui.Interfaces.General;

/// <summary>
///     Represents an object that holds a dialog result value.
/// </summary>
public interface IDialogResult
{
    /// <summary>
    ///     Gets or sets the result of the dialog interaction.
    /// </summary>
    DialogResult DialogResult { get; set; }
}