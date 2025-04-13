using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;
using Hashi.Gui.ViewModels;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Hashi.Gui.Providers
{

    /// <inheritdoc cref="IHashiSettingsProvider"/>
    public class HashiSettingsProvider : IHashiSettingsProvider
    {
        private readonly IJsonWrapper jsonWrapper;
        private readonly Func<ISettingsViewModel> settingsFactory;

        /// <inheritdoc cref="IHashiSettingsProvider"/>
        public HashiSettingsProvider(IJsonWrapper jsonWrapper, Func<ISettingsViewModel> settingsFactory)
        {
            this.jsonWrapper = jsonWrapper;
            this.settingsFactory = settingsFactory;
            Settings = LoadSettings();
        }

        /// <inheritdoc />
        public ISettingsViewModel Settings { get; }

        /// <inheritdoc />
        public string HashiSettingsFilePath => Path.Combine(SettingsDirectoryPath, HashiSettingsFileName);

        /// <inheritdoc />
        public string SettingsDirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), LocalAppDataPath);

        /// <inheritdoc />
        public string HashiSettingsFileName => "HashiSettings.json";

        /// <inheritdoc />
        public string LocalAppDataPath => @"CN_Hashi\Settings";

        public ISettingsViewModel LoadSettings()
        {
            ISettingsViewModel loadedSettings;

            try
            {
                var path = HashiSettingsFilePath;
                if (File.Exists(path))
                {
                    loadedSettings =
                        (SettingsViewModel)jsonWrapper.DeserializeObject(File.ReadAllText(path),
                            typeof(SettingsViewModel))!;

                    TranslationSource.Instance.CurrentCulture =
                        new CultureInfo(loadedSettings.SelectedLanguageCulture ?? "en-GB");
                    return loadedSettings;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }

            loadedSettings = settingsFactory.Invoke();
            loadedSettings.InitializeHighScores();
            loadedSettings.SelectedLanguageCulture = loadedSettings.Languages[0].Culture;
            TranslationSource.Instance.CurrentCulture = new CultureInfo(loadedSettings.SelectedLanguageCulture ?? "en-GB");

            return loadedSettings;
        }

        public void SaveSettings()
        {
            if (Settings == null) throw new InvalidOperationException("Settings cannot be null.");

            var jsonArray = jsonWrapper.SerializeObject(Settings);
            var path = HashiSettingsFilePath;
            try
            {
                if (!Directory.Exists(SettingsDirectoryPath)) Directory.CreateDirectory(SettingsDirectoryPath);

                File.WriteAllText(path, jsonArray);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }
    }
}
