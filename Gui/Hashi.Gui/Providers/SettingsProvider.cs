using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;
using Hashi.Gui.ViewModels.Settings;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="ISettingsProvider" />
public class SettingsProvider : ISettingsProvider
{
    private readonly IJsonWrapper jsonWrapper;
    private readonly Func<ISettingsViewModel> settingsFactory;
    private readonly IPathProvider pathProvider;

    /// <inheritdoc cref="ISettingsProvider" />
    public SettingsProvider(IJsonWrapper jsonWrapper, Func<ISettingsViewModel> settingsFactory, IPathProvider pathProvider)
    {
        this.jsonWrapper = jsonWrapper;
        this.settingsFactory = settingsFactory;
        this.pathProvider = pathProvider;
        Settings = LoadSettings();
    }

    /// <inheritdoc />
    public ISettingsViewModel Settings { get; }

    /// <inheritdoc />
    public void SaveSettings()
    {
        if (Settings == null) throw new InvalidOperationException("Settings cannot be null.");

        var jsonArray = jsonWrapper.SerializeObject(Settings);
        var path = pathProvider.HashiSettingsFilePath;
        try
        {
            if (!Directory.Exists(pathProvider.SettingsDirectoryPath)) Directory.CreateDirectory(pathProvider.SettingsDirectoryPath);

            File.WriteAllText(path, jsonArray);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }
    }

    public ISettingsViewModel LoadSettings()
    {
        ISettingsViewModel loadedSettings;

        try
        {
            var path = pathProvider.HashiSettingsFilePath;
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
}