using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Hashi.Gui.ViewModels
{
    /// <summary>
    /// Represents the view model for the settings, including high scores for different difficulty levels.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SettingsViewModel : BaseViewModel
    {
        private bool areGridLinesEnabled;

        /// <summary>
        /// Determines whether the grid lines are enabled in the game view.
        /// </summary>
        [JsonProperty(nameof(AreGridLinesEnabled))]
        public bool AreGridLinesEnabled
        {
            get => areGridLinesEnabled;
            set => Set(ref areGridLinesEnabled, value);
        }

        /// <summary>
        /// Gets the collection of high scores for different difficulty levels.
        /// </summary>
        [JsonProperty(nameof(HighScores))]
        public ObservableCollection<HighScorePerDifficultyViewModel> HighScores { get; } = new();

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Join(" | ", $"HighScores: {string.Join(", ", HighScores)}");
        }
    }
}
