using Hashi.Enums;
using Hashi.Gui.Interfaces.Messages;
using System.Diagnostics;
using System.Windows.Input;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///   The main view model for the Hashi game.
/// </summary>
public interface IMainViewModel
{
    /// <summary>
    ///     The connection manager for managing the connections between islands.
    /// </summary>
    public IConnectionManagerViewModel ConnectionManager { get; }

    /// <summary>
    /// Determines if a hashi puzzle is being generated.
    /// </summary>
    bool IsGeneratingHashiPuzzle { get; set; }

    /// <summary>
    ///     Determines if the timer is running.
    /// </summary>
    bool IsTimerRunning { get; set; }

    /// <summary>
    /// Determines if the user has used hints or not.
    /// </summary>
    bool IsCheating { get; set; }

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
    ///    Command to generate a hint.
    /// </summary>
    ICommand GenerateHintCommand { get; }

    /// <summary>
    /// Command executed when the window is clicked anywhere
    /// </summary>
    ICommand WindowMouseClickedCommand { get; }

    /// <summary>
    ///   Command executed when the user clicks on the "Undo" button.
    /// </summary>
    ICommand UndoCommand { get; }

    /// <summary>
    ///    Command executed when the user clicks on the "Redo" button.
    /// </summary>
    ICommand RedoCommand { get; }

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
    Task CreateNewGameAsync();

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
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ReceiveAsync(IAllConnectionsSetMessage message, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the message when the potential target island is changed.
    /// </summary>
    /// <param name="islandChangedMessage">The <see cref="IDropTargetIslandChangedMessage" />.</param>
    void Receive(IDropTargetIslandChangedMessage islandChangedMessage);
}