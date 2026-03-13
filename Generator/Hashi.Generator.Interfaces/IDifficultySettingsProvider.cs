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
    /// <returns>A tuple containing min/max length/width, divisor, alpha, and beta values.</returns>
    (int minLength, int maxLength, int minWidth, int maxWidth, int divisor, int alpha, int beta) GetDifficultySettings(
        int difficulty);

    /// <summary>
    ///     Gets the alpha value for a given difficulty level.
    /// </summary>
    /// <param name="difficulty">The difficulty level (0-9).</param>
    /// <returns>The alpha value.</returns>
    int GetAlphaForDifficulty(int difficulty);

    /// <summary>
    ///     Gets the beta value for a given difficulty level.
    /// </summary>
    /// <param name="difficulty">The difficulty level (0-9).</param>
    /// <returns>The beta value.</returns>
    int GetBetaForDifficulty(int difficulty);
}
