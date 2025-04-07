using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Represents the view model for the settings, including high scores for different difficulty levels.
/// </summary>
public interface ISettingsViewModel : IBaseViewModel
{
    /// <summary>
    ///     Determines whether the grid lines are enabled in the game view.
    /// </summary>
    bool AreGridLinesEnabled { get; set; }

    /// <summary>
    ///     Gets the collection of high scores for different difficulty levels.
    /// </summary>
    ObservableCollection<IHighScorePerDifficultyViewModel> HighScores { get; }
}