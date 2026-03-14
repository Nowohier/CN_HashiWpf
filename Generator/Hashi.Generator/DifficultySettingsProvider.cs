using Hashi.Generator.Interfaces;

namespace Hashi.Generator;

/// <inheritdoc />
public class DifficultySettingsProvider : IDifficultySettingsProvider
{
    /// <inheritdoc />
    public DifficultySettings GetDifficultySettings(int difficulty)
    {
        return difficulty switch
        {
            0 => new DifficultySettings(5, 10, 5, 10, 4, 25, 20),
            1 => new DifficultySettings(14, 16, 14, 16, 4, 50, 20),
            2 => new DifficultySettings(10, 16, 10, 16, 3, 75, 20),
            3 => new DifficultySettings(11, 18, 11, 18, 3, 25, 15),
            4 => new DifficultySettings(10, 18, 10, 18, 3, 50, 15),
            5 => new DifficultySettings(13, 18, 13, 18, 3, 75, 15),
            6 => new DifficultySettings(15, 20, 15, 20, 3, 25, 10),
            7 => new DifficultySettings(14, 20, 14, 20, 3, 50, 10),
            8 => new DifficultySettings(16, 31, 16, 31, 3, 75, 10),
            9 => new DifficultySettings(20, 31, 20, 31, 3, 100, 0),
            _ => throw new ArgumentException("Invalid difficulty level.")
        };
    }

    /// <inheritdoc />
    public int GetAlphaForDifficulty(int difficulty)
    {
        return difficulty switch
        {
            0 or 3 or 6 => 25,
            1 or 4 or 7 => 50,
            2 or 5 or 8 => 75,
            9 => 100,
            _ => 0
        };
    }

    /// <inheritdoc />
    public int GetBetaForDifficulty(int difficulty)
    {
        return difficulty switch
        {
            <= 2 => 20,
            <= 5 => 15,
            <= 8 => 10,
            _ => 0
        };
    }
}
