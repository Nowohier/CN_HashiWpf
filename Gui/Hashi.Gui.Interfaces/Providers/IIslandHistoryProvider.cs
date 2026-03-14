using Hashi.Gui.Interfaces.Models;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides undo/redo history operations for island connections.
/// </summary>
public interface IIslandHistoryProvider
{
    /// <summary>
    ///     The history of all connections made in the game.
    /// </summary>
    IList<IHashiBridge> History { get; }

    /// <summary>
    ///     The redo history of undone connections.
    /// </summary>
    IList<IHashiBridge> RedoHistory { get; }

    /// <summary>
    ///     Undo the last connection made in the game.
    /// </summary>
    void UndoConnection();

    /// <summary>
    ///     Redo the last undone connection.
    /// </summary>
    void RedoConnection();
}
