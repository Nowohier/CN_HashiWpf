using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    /// Interface for providing settings file paths and names.
    /// </summary>
    public interface IHashiSettingsProvider
    {
        /// <summary>
        /// Gets the settings view model.
        /// </summary>
        ISettingsViewModel Settings { get; }

        /// <summary>
        /// Gets the complete path to the settings file.
        /// </summary>
        string HashiSettingsFilePath { get; }

        /// <summary>
        /// Gets the path to the settings file directory.
        /// </summary>
        string SettingsDirectoryPath { get; }

        /// <summary>
        /// Gets the name of the settings file.
        /// </summary>
        string HashiSettingsFileName { get; }

        /// <summary>
        /// Gets the path to the local application data hashi settings directory.
        /// </summary>
        string LocalAppDataPath { get; }

        /// <summary>
        /// Loads the settings from a JSON file.
        /// </summary>
        /// <returns>an <see cref="ISettingsViewModel"/>.</returns>
        ISettingsViewModel LoadSettings();

        /// <summary>
        ///     Saves the settings to the JSON file.
        /// </summary>
        void SaveSettings();
    }
}
