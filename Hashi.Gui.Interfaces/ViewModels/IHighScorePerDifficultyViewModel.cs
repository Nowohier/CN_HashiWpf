using Hashi.Enums;

namespace Hashi.Gui.Interfaces.ViewModels;

public interface IHighScorePerDifficultyViewModel
{
    /// <summary>
    ///     Gets or sets the high score time for the specified difficulty level.
    /// </summary>
    TimeSpan? HighScoreTime { get; set; }

    /// <summary>
    ///     Gets the difficulty level associated with this high score.
    /// </summary>
    DifficultyEnum Difficulty { get; }
}