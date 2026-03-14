using Hashi.Enums;

namespace Hashi.Gui.Interfaces.Services;

/// <summary>
///     Handles game completion logic including highscore checks and dialogs.
/// </summary>
public interface IGameCompletionHandler
{
    /// <summary>
    ///     Handles game completion, checking highscores and showing dialogs.
    /// </summary>
    /// <param name="score">The player's elapsed time.</param>
    /// <param name="difficulty">The difficulty level of the completed game.</param>
    /// <param name="isCheating">Whether the player used hints.</param>
    /// <param name="isTestFieldMode">Whether the game was in test field mode.</param>
    /// <returns><c>true</c> if a new game should be started.</returns>
    bool HandleGameCompletion(TimeSpan score, DifficultyEnum difficulty, bool isCheating, bool isTestFieldMode);
}
