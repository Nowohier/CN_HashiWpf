using Hashi.Gui.Interfaces.Providers;
using System.IO;

namespace Hashi.Gui.Providers
{

    /// <inheritdoc cref="IPathProvider"/>
    public class PathProvider : IPathProvider
    {
        /// <inheritdoc />
        public string HashiSettingsFilePath => Path.Combine(SettingsDirectoryPath, HashiSettingsFileName);

        /// <inheritdoc />
        public string HashiTestFieldsFilePath => Path.Combine(SettingsDirectoryPath, HashiTestFieldsFileName);

        /// <inheritdoc />
        public string SettingsDirectoryPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), LocalAppDataPath);

        /// <inheritdoc />
        public string LocalAppDataPath => @"CN_Hashi\Settings";

        /// <summary>
        /// Gets the name of the settings file.
        /// </summary>
        private string HashiSettingsFileName => "HashiSettings.json";

        /// <summary>
        /// Gets the name of the test fields file.
        /// </summary>
        private string HashiTestFieldsFileName => "HashiTestfields.json";
    }
}
