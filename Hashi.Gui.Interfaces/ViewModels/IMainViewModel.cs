using System.Diagnostics;
using System.Windows.Input;
using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Interfaces.ViewModels;

public interface IMainViewModel : IBaseViewModel
{
    /// <summary>
    ///     The connection manager for managing the connections between islands.
    /// </summary>
    public IConnectionManagerViewModel ConnectionManager { get; }

    /// <summary>
    ///     The current source island.
    /// </summary>
    IIslandViewModel? CurrentSourceIsland { get; set; }

    /// <summary>
    ///     Gets or sets the potential target island.
    /// </summary>
    IIslandViewModel? PotentialTargetIsland { get; set; }

    /// <summary>
    ///     Determines if the timer is running.
    /// </summary>
    bool IsTimerRunning { get; set; }

    /// <summary>
    ///     The selected difficulty level.
    /// </summary>
    DifficultyEnum SelectedDifficulty { get; set; }

    /// <summary>
    ///     The Hashi settings.
    /// </summary>
    ISettingsViewModel Settings { get; }

    /// <summary>
    ///     The highscore for the selected difficulty level.
    /// </summary>
    TimeSpan? HighscoreForSelectedDifficulty { get; }

    /// <summary>
    ///     Command to create a new game.
    /// </summary>
    ICommand CreateNewGameCommand { get; }

    /// <summary>
    ///     Command to remove all bridges.
    /// </summary>
    ICommand RemoveAllBridgesCommand { get; }

    /// <summary>
    ///     The current source island.
    /// </summary>
    Stopwatch Timer { get; }

    /// <summary>
    ///     Loads the settings from the JSON file.
    /// </summary>
    ISettingsViewModel LoadSettings();

    /// <summary>
    ///     Saves the settings to the JSON file.
    /// </summary>
    void SaveSettings();

    /// <summary>
    ///     Creates a new game.
    /// </summary>
    void CreateNewGame();

    /// <summary>
    ///     Creates a new game.
    /// </summary>
    void RemoveAllBridgesExecute();

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
    void Receive(IAllConnectionsSetMessage message);

    /// <summary>
    ///     Handles the message when the current source island is changed.
    /// </summary>
    /// <param name="message">The <see cref="ICurrentSourceIslandChangedMessage" />.</param>
    void Receive(ICurrentSourceIslandChangedMessage message);

    /// <summary>
    ///     Handles the message when the potential target island is changed.
    /// </summary>
    /// <param name="islandChangedMessage">The <see cref="IPotentialTargetIslandChangedMessage" />.</param>
    void Receive(IPotentialTargetIslandChangedMessage islandChangedMessage);
}