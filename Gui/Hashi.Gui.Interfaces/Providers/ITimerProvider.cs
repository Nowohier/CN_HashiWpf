using System.Diagnostics;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides functionality to manage a timer for the game.
/// </summary>
public interface ITimerProvider
{
    /// <summary>
    ///     Determines if the timer is running.
    /// </summary>
    bool IsTimerRunning { get; }

    /// <summary>
    ///     Gets the timer for the game.
    /// </summary>
    Stopwatch Timer { get; }

    /// <summary>
    ///     Starts the timer for the game.
    /// </summary>
    void StartTimer();

    /// <summary>
    ///     Stops the timer for the game and resets it.
    /// </summary>
    void StopTimer();
}