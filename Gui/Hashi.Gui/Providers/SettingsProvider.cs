using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Gui.Translation;
using Hashi.Gui.ViewModels.Settings;
using Hashi.Logging.Interfaces;
using System.Globalization;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="ISettingsProvider" />
public class SettingsProvider : ISettingsProvider
{
    private readonly IJsonWrapper jsonWrapper;
    private readonly Func<ISettingsViewModel> settingsFactory;
    private readonly IPathProvider pathProvider;
    private readonly IFileWrapper fileWrapper;
    private readonly IDirectoryWrapper directoryWrapper;
    private readonly ILogger logger;

    /// <inheritdoc cref="ISettingsProvider" />
    public SettingsProvider(IJsonWrapper jsonWrapper, Func<ISettingsViewModel> settingsFactory, IPathProvider pathProvider, ILoggerFactory loggerFactory, IFileWrapper fileWrapper, IDirectoryWrapper directoryWrapper)
    {
        this.jsonWrapper = jsonWrapper ?? throw new ArgumentNullException(nameof(jsonWrapper));
        this.settingsFactory = settingsFactory ?? throw new ArgumentNullException(nameof(settingsFactory));
        this.pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        this.fileWrapper = fileWrapper ?? throw new ArgumentNullException(nameof(fileWrapper));
        this.directoryWrapper = directoryWrapper ?? throw new ArgumentNullException(nameof(directoryWrapper));
        this.logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger<SettingsProvider>();
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
            if (!directoryWrapper.Exists(pathProvider.SettingsDirectoryPath)) directoryWrapper.CreateDirectory(pathProvider.SettingsDirectoryPath);

            fileWrapper.WriteAllText(path, jsonArray);
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
            if (fileWrapper.Exists(path))
            {
                loadedSettings =
                    (ISettingsViewModel)jsonWrapper.DeserializeObject(fileWrapper.ReadAllText(path),
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