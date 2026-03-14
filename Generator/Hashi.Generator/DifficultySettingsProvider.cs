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
            // Easy
            0 => new DifficultySettings(MinLength: 5, MaxLength: 10, MinWidth: 5, MaxWidth: 10, Divisor: 4, Alpha: 25, Beta: 20),
            1 => new DifficultySettings(MinLength: 14, MaxLength: 16, MinWidth: 14, MaxWidth: 16, Divisor: 4, Alpha: 50, Beta: 20),
            2 => new DifficultySettings(MinLength: 10, MaxLength: 16, MinWidth: 10, MaxWidth: 16, Divisor: 3, Alpha: 75, Beta: 20),
            // Medium
            3 => new DifficultySettings(MinLength: 11, MaxLength: 18, MinWidth: 11, MaxWidth: 18, Divisor: 3, Alpha: 25, Beta: 15),
            4 => new DifficultySettings(MinLength: 10, MaxLength: 18, MinWidth: 10, MaxWidth: 18, Divisor: 3, Alpha: 50, Beta: 15),
            5 => new DifficultySettings(MinLength: 13, MaxLength: 18, MinWidth: 13, MaxWidth: 18, Divisor: 3, Alpha: 75, Beta: 15),
            // Hard
            6 => new DifficultySettings(MinLength: 15, MaxLength: 20, MinWidth: 15, MaxWidth: 20, Divisor: 3, Alpha: 25, Beta: 10),
            7 => new DifficultySettings(MinLength: 14, MaxLength: 20, MinWidth: 14, MaxWidth: 20, Divisor: 3, Alpha: 50, Beta: 10),
            8 => new DifficultySettings(MinLength: 16, MaxLength: 31, MinWidth: 16, MaxWidth: 31, Divisor: 3, Alpha: 75, Beta: 10),
            // Expert
            9 => new DifficultySettings(MinLength: 20, MaxLength: 31, MinWidth: 20, MaxWidth: 31, Divisor: 3, Alpha: 100, Beta: 0),
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
