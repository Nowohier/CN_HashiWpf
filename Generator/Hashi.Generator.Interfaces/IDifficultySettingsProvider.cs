namespace Hashi.Generator.Interfaces;

/// <summary>
///     Provides difficulty-related settings for puzzle generation.
/// </summary>
public interface IDifficultySettingsProvider
{
    /// <summary>
    ///     Gets the preconfigured settings for a given difficulty level.
    /// </summary>
    /// <param name="difficulty">The difficulty level (0-9).</param>
    /// <returns>The difficulty settings for the given level.</returns>
    DifficultySettings GetDifficultySettings(int difficulty);
}
