namespace Hashi.Gui.Interfaces.Views;

/// <summary>
///     Provides access to the view box control used for coordinate transformations.
/// </summary>
public interface IViewBoxControl
{
    /// <summary>
    ///     Gets the underlying view box control element.
    /// </summary>
    object ViewBoxControl { get; }
}