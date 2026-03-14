namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Provides functionality to manage a timer for the game.
/// </summary>
public interface ITimerProvider
{
    /// <summary>
    ///     Gets the elapsed time since the timer was started.
    /// </summary>
    TimeSpan Elapsed { get; }

    /// <summary>
    ///     Gets a value indicating whether the timer is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    ///     Starts the timer for the game.
    /// </summary>
    void StartTimer();

    /// <summary>
    ///     Stops the timer for the game and resets it.
    /// </summary>
    void StopTimer();

    /// <summary>
    ///     Gets a value indicating whether the timer is running.
    /// </summary>
    bool IsTimerRunning { get; }
}