using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Interface for providing settings file paths and names.
/// </summary>
public interface IHashiSettingsProvider
{
    /// <summary>
    ///     Gets the settings view model.
    /// </summary>
    ISettingsViewModel Settings { get; }

    /// <summary>
    ///     Saves the settings to the JSON file.
    /// </summary>
    void SaveSettings();
}