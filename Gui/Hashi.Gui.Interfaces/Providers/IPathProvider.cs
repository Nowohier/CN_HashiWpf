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
        ///  Gets the path to the local application data directory.
        /// </summary>
        string LocalAppDataPath { get; }
    }
}
