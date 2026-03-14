using Hashi.Enums;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Services;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;

namespace Hashi.Gui.Services;

/// <inheritdoc />
public class GameCompletionHandler : IGameCompletionHandler
{
    private readonly IDialogWrapper dialogWrapper;
    private readonly ISettingsProvider settingsProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameCompletionHandler" /> class.
    /// </summary>
    /// <param name="dialogWrapper">The dialog wrapper.</param>
    /// <param name="settingsProvider">The settings provider.</param>
    public GameCompletionHandler(IDialogWrapper dialogWrapper, ISettingsProvider settingsProvider)
    {
        this.dialogWrapper = dialogWrapper ?? throw new ArgumentNullException(nameof(dialogWrapper));
        this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
    }

    /// <inheritdoc />
    public bool HandleGameCompletion(TimeSpan score, DifficultyEnum difficulty, bool isCheating, bool isTestFieldMode)
    {
        var caption = TranslationSource.Instance.GetRequired("MessageGameOverCaption");
        var dialogMessage = string.Format(
            TranslationSource.Instance.GetRequired("MessageGameOverText"),
            score.ToString(@"hh\:mm\:ss"));

        if (isCheating || isTestFieldMode)
        {
            dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);
            return !isTestFieldMode;
        }

        var currentSettingForSetDifficulty =
            settingsProvider.Settings.HighScores.FirstOrDefault(x => x.Difficulty == difficulty);
        var currentHighScore = currentSettingForSetDifficulty?.HighScoreTime;

        if (currentSettingForSetDifficulty != null && (currentHighScore == null || score < currentHighScore))
        {
            caption = TranslationSource.Instance.GetRequired("MessageNewHighscoreCaption");
            dialogMessage += string.Format(
                TranslationSource.Instance.GetRequired("MessageNewHighscoreText")
                    .Replace(@"\n", Environment.NewLine),
                difficulty.ToString(), score.ToString(@"hh\:mm\:ss"),
                currentHighScore == null ? "-" : ((TimeSpan)currentHighScore).ToString(@"hh\:mm\:ss"));
            currentSettingForSetDifficulty.HighScoreTime = score;
            settingsProvider.SaveSettings();
        }

        dialogWrapper.Show(caption, dialogMessage, DialogButton.Ok, DialogImage.Success);
        return true;
    }
}
