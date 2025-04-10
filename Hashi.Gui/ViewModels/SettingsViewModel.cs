using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="ISettingsViewModel" />
[JsonObject(MemberSerialization.OptIn)]
public class SettingsViewModel : ObservableRecipient, ISettingsViewModel
{
    private bool areGridLinesEnabled;
    private string? selectedLanguage;

    public SettingsViewModel()
    {
        Initialize();
    }

    /// <inheritdoc />
    [JsonProperty(nameof(AreGridLinesEnabled))]
    public bool AreGridLinesEnabled
    {
        get => areGridLinesEnabled;
        set => SetProperty(ref areGridLinesEnabled, value);
    }

    /// <inheritdoc />
    [JsonProperty(nameof(SelectedLanguageCulture))]
    public string? SelectedLanguageCulture
    {
        get => selectedLanguage;
        set => SetProperty(ref selectedLanguage, value);
    }

    /// <inheritdoc />
    [JsonProperty(nameof(HighScores))]
    public ObservableCollection<IHighScorePerDifficultyViewModel> HighScores { get; } = new();

    /// <inheritdoc />
    public ObservableCollection<ILanguageViewModel> Languages { get; } = new();

    /// <inheritdoc />
    public void Initialize()
    {
        HighScores.Clear();
        HighScores.AddRange(Enum.GetValues<DifficultyEnum>()
            .Select(x => new HighScorePerDifficultyViewModel(x)));

        Languages.Clear();
        Languages.Add(new LanguageViewModel("English", "English", "en-GB"));
        Languages.Add(new LanguageViewModel("German", "Deutsch", "de-DE"));
        SelectedLanguageCulture = Languages[0].Culture;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(" | ", $"HighScores: {string.Join(", ", HighScores)}");
    }
}