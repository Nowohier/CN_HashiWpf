namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Represents the view model for an island in the Hashi game.
///     Composite of <see cref="IIslandData" />, <see cref="IIslandHighlightState" />,
///     <see cref="IIslandBridgeData" />, <see cref="IIslandDropTarget" />,
///     and <see cref="IIslandColorManager" />.
/// </summary>
public interface IIslandViewModel : IIslandData, IIslandHighlightState, IIslandBridgeData, IIslandDropTarget,
    IIslandColorManager
{
}
