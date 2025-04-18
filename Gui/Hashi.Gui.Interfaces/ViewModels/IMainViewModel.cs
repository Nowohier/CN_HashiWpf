using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     The main view model for the Hashi game.
/// </summary>
public interface IMainViewModel
{
    /// <summary>
    ///     The settings provider for the Hashi game.
    /// </summary>
    ISettingsProvider SettingsProvider { get; }

    /// <summary>
    ///     Creates a new game asynchronously.
    /// </summary>
    Task CreateNewGameAsync();

    /// <summary>
    ///     Handles the message when a bridge connection is changed.
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="ArgumentOutOfRangeException">The <see cref="IBridgeConnectionChangedMessage" />.</exception>
    void Receive(IBridgeConnectionChangedMessage message);

    /// <summary>
    ///     Handles the message when all connections are set.
    /// </summary>
    /// <param name="message">The <see cref="IAllConnectionsSetMessage" />.</param>
    void Receive(IAllConnectionsSetMessage message);
}