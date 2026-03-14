namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides island management operations for the Hashi game board.
///     Composes all focused sub-interfaces for island data, connections, UI, history, and initialization.
/// </summary>
public interface IIslandProvider :
    IIslandDataProvider,
    IIslandConnectionProvider,
    IIslandUiProvider,
    IIslandHistoryProvider,
    IIslandInitializationProvider
{
}
