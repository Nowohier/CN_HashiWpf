namespace Hashi.Generator.Interfaces;

/// <summary>
///     Represents the settings for a specific difficulty level.
/// </summary>
/// <param name="MinLength">The minimum field length.</param>
/// <param name="MaxLength">The maximum field length.</param>
/// <param name="MinWidth">The minimum field width.</param>
/// <param name="MaxWidth">The maximum field width.</param>
/// <param name="Divisor">The divisor used to calculate the number of islands.</param>
/// <param name="Alpha">The alpha value controlling additional bridge density.</param>
/// <param name="Beta">The beta value controlling double bridge probability.</param>
public record DifficultySettings(int MinLength, int MaxLength, int MinWidth, int MaxWidth, int Divisor, int Alpha, int Beta);
