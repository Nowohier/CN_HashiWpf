using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Interface for providing settings file paths and names.
/// </summary>
public interface ISettingsProvider
{
    /// <summary>
    ///     Gets the settings view model.
    /// </summary>
    ISettingsViewModel Settings { get; }

    /// <summary>
    ///     Saves the settings to the JSON file.
    /// </summary>
    void SaveSettings();

    /// <summary>
    ///   Resets the settings to the default values.
    /// </summary>
    void ResetSettings();

    /// <summary>
    ///    Loads the settings from the JSON file.
    /// </summary>
    /// <returns>a <see cref="ISettingsViewModel"/>.</returns>
    ISettingsViewModel LoadSettings();
}