using CNHashiWpf.Enums;
using Newtonsoft.Json;

namespace CNHashiWpf.ViewModels
{
    /// <summary>
    /// Represents a view model for displaying high scores per difficulty level.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class HighScorePerDifficultyViewModel : BaseViewModel
    {
        private TimeSpan? highScoreTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="HighScorePerDifficultyViewModel"/> class.
        /// </summary>
        /// <param name="difficulty">The difficulty setting.</param>
        public HighScorePerDifficultyViewModel(DifficultyEnum difficulty)
        {
            Difficulty = difficulty;
        }

        /// <summary>
        /// Gets or sets the high score time for the specified difficulty level.
        /// </summary>
        [JsonProperty(nameof(HighScoreTime))]
        public TimeSpan? HighScoreTime
        {
            get => highScoreTime;
            set => Set(ref highScoreTime, value);
        }

        /// <summary>
        /// Gets the difficulty level associated with this high score.
        /// </summary>
        [JsonProperty(nameof(Difficulty))]
        public DifficultyEnum Difficulty { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(" | ", $"Difficulty: {Difficulty}", $"HighScoreTime: {(HighScoreTime is { } span ? span.ToString(@"hh\:mm\:ss") : "-")}");
        }
    }
}
