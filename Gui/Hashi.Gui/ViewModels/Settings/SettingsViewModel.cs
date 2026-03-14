using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Enums;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Hashi.Gui.ViewModels.Settings;

/// <inheritdoc cref="ISettingsViewModel" />
[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class SettingsViewModel : ObservableObject, ISettingsViewModel
{
    private bool areGridLinesEnabled;
    private string? selectedLanguage;

    public SettingsViewModel()
    {
        InitializeLanguages();
    }

    /// <inheritdoc />
    [JsonPropertyName("AreGridLinesEnabled")]
    public bool AreGridLinesEnabled
    {
        get => areGridLinesEnabled;
        set => SetProperty(ref areGridLinesEnabled, value);
    }

    /// <inheritdoc />
    [JsonPropertyName("SelectedLanguageCulture")]
    public string? SelectedLanguageCulture
    {
        get => selectedLanguage ?? (Languages.Count > 0 ? Languages.First().Culture : null);
        set => SetProperty(ref selectedLanguage, value);
    }

    /// <inheritdoc />
    [JsonPropertyName("HighScores")]
    public ObservableCollection<IHighScorePerDifficultyViewModel> HighScores { get; } = [];

    /// <inheritdoc />
    [JsonIgnore]
    public ObservableCollection<ILanguageViewModel> Languages { get; } = [];

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
