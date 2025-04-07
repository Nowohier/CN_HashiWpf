using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.ViewModels;
using Newtonsoft.Json;

namespace Hashi.Gui.ViewModels;

/// <summary>
///     Represents a view model for displaying high scores per difficulty level.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class HighScorePerDifficultyViewModel : ObservableRecipient, IHighScorePerDifficultyViewModel
{
    private TimeSpan? highScoreTime;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HighScorePerDifficultyViewModel" /> class.
    /// </summary>
    /// <param name="difficulty">The difficulty setting.</param>
    public HighScorePerDifficultyViewModel(DifficultyEnum difficulty)
    {
        Difficulty = difficulty;
    }

    /// <inheritdoc />
    [JsonProperty(nameof(HighScoreTime))]
    public TimeSpan? HighScoreTime
    {
        get => highScoreTime;
        set => SetProperty(ref highScoreTime, value);
    }

    /// <inheritdoc />
    [JsonProperty(nameof(Difficulty))]
    public DifficultyEnum Difficulty { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(" | ", $"Difficulty: {Difficulty}",
            $"HighScoreTime: {(HighScoreTime is { } span ? span.ToString(@"hh\:mm\:ss") : "-")}");
    }
}