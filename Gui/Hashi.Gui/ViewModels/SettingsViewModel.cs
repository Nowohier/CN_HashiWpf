using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.ViewModels;
using Newtonsoft.Json;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="ISettingsViewModel" />
[JsonObject(MemberSerialization.OptIn)]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class SettingsViewModel : ObservableRecipient, ISettingsViewModel
{
    private bool areGridLinesEnabled;
    private string? selectedLanguage;

    public SettingsViewModel()
    {
        InitializeLanguages();
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
        get => selectedLanguage ?? (Languages.Count > 0 ? Languages.First().Culture : null);
        set => SetProperty(ref selectedLanguage, value);
    }

    /// <inheritdoc />
    [JsonProperty(nameof(HighScores))]
    public ObservableCollection<IHighScorePerDifficultyViewModel> HighScores { get; } = new();

    /// <inheritdoc />
    public ObservableCollection<ILanguageViewModel> Languages { get; } = new();

    /// <inheritdoc />
    public void InitializeHighScores()
    {
        HighScores.Clear();
        HighScores.AddRange(Enum.GetValues<DifficultyEnum>()
            .Select(x => new HighScorePerDifficultyViewModel(x)));
    }

    private void InitializeLanguages()
    {
        Languages.Clear();
        Languages.Add(new LanguageViewModel("English", "English", "en-GB"));
        Languages.Add(new LanguageViewModel("German", "Deutsch", "de-DE"));
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(" | ", $"HighScores: {string.Join(", ", HighScores)}");
    }
}