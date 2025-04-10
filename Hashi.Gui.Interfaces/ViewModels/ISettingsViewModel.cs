using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.ViewModels;

/// <summary>
///     Represents the view model for the settings, including high scores for different difficulty levels.
/// </summary>
public interface ISettingsViewModel
{
    /// <summary>
    ///     Determines whether the grid lines are enabled in the game view.
    /// </summary>
    bool AreGridLinesEnabled { get; set; }

    /// <summary>
    ///     Gets the collection of high scores for different difficulty levels.
    /// </summary>
    ObservableCollection<IHighScorePerDifficultyViewModel> HighScores { get; }

    /// <summary>
    ///    Gets the collection of available languages for the game.
    /// </summary>
    ObservableCollection<ILanguageViewModel> Languages { get; }

    /// <summary>
    ///    Gets or sets the selected language culture for the game.
    /// </summary>
    string? SelectedLanguageCulture { get; set; }

    /// <summary>
    ///   Initializes a fresh settings configuration.
    /// </summary>
    void InitializeHighScores();
}