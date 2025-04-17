namespace Hashi.Gui.Interfaces.Views;

public interface IWindow : IViewBoxControl
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