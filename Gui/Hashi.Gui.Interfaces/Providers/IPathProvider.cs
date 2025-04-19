namespace Hashi.Gui.Interfaces.Providers
{
    /// <summary>
    ///  Provides paths for the settings file and directory.
    /// </summary>
    public interface IPathProvider
    {
        /// <summary>
        ///   Gets the path to the settings file.
        /// </summary>
        string HashiSettingsFilePath { get; }

        /// <summary>
        ///  Gets the path to the test fields file.
        /// </summary>
        string HashiTestFieldsFilePath { get; }

        /// <summary>
        ///  Gets the path to the settings directory.
        /// </summary>
        string SettingsDirectoryPath { get; }

        /// <summary>
        /// Gets the hashi settings file name.
        /// </summary>
        string HashiSettingsFileName { get; }

        /// <summary>
        /// Gets the hashi test fields file name.
        /// </summary>
        string HashiTestFieldsFileName { get; }
    }
}
