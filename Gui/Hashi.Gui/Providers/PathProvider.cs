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

        /// <summary>
        /// Gets the local application data path.
        /// </summary>
        public string LocalAppDataPath => @"CN_Hashi\Settings";

        /// <inheritdoc />
        public string HashiSettingsFileName => "HashiSettings.json";

        /// <inheritdoc />
        public string HashiTestFieldsFileName => "HashiTestfields.json";
    }
}
