namespace Hashi.Gui.Interfaces.Views;

/// <summary>
///     Interface for the main view of the Hashi application.
/// </summary>
public interface IHashiMainView : IViewBoxControl
{
    /// <summary>
    ///     Gets or sets the view model for the main view.
    /// </summary>
    public object DataContext { get; set; }

    /// <summary>
    ///     Show the window as dialog.
    /// </summary>
    public bool? ShowDialog();
}