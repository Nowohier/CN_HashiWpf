using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     The main view model for the Hashi game.
/// </summary>
public interface IMainViewModel
{
    /// <summary>
    /// The timer provider for the Hashi game.
    /// </summary>
    ITimerProvider TimerProvider { get; }

    /// <summary>
    /// The settings provider for the Hashi game.
    /// </summary>
    IHashiSettingsProvider SettingsProvider { get; }

    /// <summary>
    ///     Creates a new game.
    /// </summary>
    Task CreateNewGameAsync();

    /// <summary>
    ///     Handles the message when a bridge connection is changed.
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="ArgumentOutOfRangeException">The <see cref="IBridgeConnectionChangedMessage" />.</exception>
    void Receive(IBridgeConnectionChangedMessage message);

    /// <summary>
    ///     Updates the color of all islands.
    /// </summary>
    /// <param name="message">The <see cref="IUpdateAllIslandColorsMessage" />.</param>
    void Receive(IUpdateAllIslandColorsMessage message);

    /// <summary>
    ///     Handles the message when all connections are set.
    /// </summary>
    /// <param name="message">The <see cref="IAllConnectionsSetMessage" />.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ReceiveAsync(IAllConnectionsSetMessage message, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the message when the potential target island is changed.
    /// </summary>
    /// <param name="islandChangedMessage">The <see cref="IDropTargetIslandChangedMessage" />.</param>
    void Receive(IDropTargetIslandChangedMessage islandChangedMessage);
}