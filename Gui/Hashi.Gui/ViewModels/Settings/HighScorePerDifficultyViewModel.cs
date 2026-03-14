using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;
using System.Text.Json.Serialization;

namespace Hashi.Gui.ViewModels.Settings;

/// <summary>
///     Represents a view model for displaying high scores per difficulty level.
/// </summary>
public class HighScorePerDifficultyViewModel : ObservableObject, IHighScorePerDifficultyViewModel
{
    private TimeSpan? highScoreTime;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HighScorePerDifficultyViewModel" /> class.
    /// </summary>
    /// <param name="difficulty">The difficulty setting.</param>
    [JsonConstructor]
    public HighScorePerDifficultyViewModel(DifficultyEnum difficulty)
    {
        Difficulty = difficulty;
    }

    /// <inheritdoc />
    [JsonPropertyName("HighScoreTime")]
    public TimeSpan? HighScoreTime
    {
        get => highScoreTime;
        set => SetProperty(ref highScoreTime, value);
    }

    /// <inheritdoc />
    [JsonPropertyName("Difficulty")]
    public DifficultyEnum Difficulty { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(" | ", $"Difficulty: {Difficulty}",
            $"HighScoreTime: {(HighScoreTime is { } span ? span.ToString(@"hh\:mm\:ss") : "-")}");
    }
}
