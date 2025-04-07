using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Gui.Interfaces.ViewModels;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Hashi.Gui.ViewModels;

/// <inheritdoc cref="ISettingsViewModel" />
[JsonObject(MemberSerialization.OptIn)]
public class SettingsViewModel : ObservableRecipient, ISettingsViewModel
{
    private bool areGridLinesEnabled;

    /// <inheritdoc />
    [JsonProperty(nameof(AreGridLinesEnabled))]
    public bool AreGridLinesEnabled
    {
        get => areGridLinesEnabled;
        set => SetProperty(ref areGridLinesEnabled, value);
    }

    /// <inheritdoc />
    [JsonProperty(nameof(HighScores))]
    public ObservableCollection<IHighScorePerDifficultyViewModel> HighScores { get; } = new();

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Join(" | ", $"HighScores: {string.Join(", ", HighScores)}");
    }
}