using Hashi.Gui.Interfaces.Logging;
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
    private readonly ILogger logger;

    /// <inheritdoc cref="ISettingsProvider" />
    public SettingsProvider(IJsonWrapper jsonWrapper, Func<ISettingsViewModel> settingsFactory, IPathProvider pathProvider, ILoggerFactory loggerFactory)
    {
        this.jsonWrapper = jsonWrapper;
        this.settingsFactory = settingsFactory;
        this.pathProvider = pathProvider;
        this.logger = loggerFactory.CreateLogger<SettingsProvider>();
        Settings = LoadSettings();
        logger.Info("SettingsProvider initialized");
    }

    /// <inheritdoc />
    public ISettingsViewModel Settings { get; private set; }

    /// <inheritdoc />
    public void ResetSettings()
    {
        Settings = LoadSettings();
    }

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
            logger.Error("Failed to save settings", ex);
        }
    }

    public ISettingsViewModel LoadSettings()
    {
        logger.Debug($"Loading settings from {pathProvider.HashiSettingsFilePath}");
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
                logger.Info("Settings loaded successfully from file");
                return loadedSettings;
            }
        }
        catch (Exception ex)
        {
            logger.Error("Failed to load settings, using defaults", ex);
        }

        loadedSettings = settingsFactory.Invoke();
        loadedSettings.InitializeHighScores();
        loadedSettings.SelectedLanguageCulture = loadedSettings.Languages[0].Culture;
        TranslationSource.Instance.CurrentCulture = new CultureInfo(loadedSettings.SelectedLanguageCulture ?? "en-GB");
        logger.Info("Created new default settings");

        return loadedSettings;
    }
}