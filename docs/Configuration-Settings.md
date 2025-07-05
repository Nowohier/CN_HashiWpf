# Configuration & Settings

This document covers application configuration, settings management, and internationalization features.

## Application Settings

### Settings Architecture
The application uses a hierarchical settings system with multiple configuration sources:

1. **Default Settings**: Embedded in application resources
2. **User Settings**: Stored in user's application data folder
3. **Runtime Settings**: Temporary settings during application execution

### Settings Provider
The `ISettingsProvider` interface manages all application settings:

```csharp
public interface ISettingsProvider
{
    T GetSetting<T>(string key, T defaultValue = default);
    void SetSetting<T>(string key, T value);
    void SaveSettings();
    void LoadSettings();
    void ResetToDefaults();
    bool HasSetting(string key);
    void RemoveSetting(string key);
}
```

### Settings Categories

#### Game Settings
```csharp
public class GameSettings
{
    public const string DefaultDifficulty = "Game.DefaultDifficulty";
    public const string DefaultGridWidth = "Game.DefaultGridWidth";
    public const string DefaultGridHeight = "Game.DefaultGridHeight";
    public const string ShowHints = "Game.ShowHints";
    public const string AutoSave = "Game.AutoSave";
    public const string TimerEnabled = "Game.TimerEnabled";
    public const string SoundEnabled = "Game.SoundEnabled";
}

// Usage
var difficulty = settingsProvider.GetSetting(GameSettings.DefaultDifficulty, DifficultyEnum.Medium1);
settingsProvider.SetSetting(GameSettings.ShowHints, true);
```

#### UI Settings
```csharp
public class UISettings
{
    public const string Theme = "UI.Theme";
    public const string Language = "UI.Language";
    public const string WindowWidth = "UI.WindowWidth";
    public const string WindowHeight = "UI.WindowHeight";
    public const string WindowLeft = "UI.WindowLeft";
    public const string WindowTop = "UI.WindowTop";
    public const string WindowMaximized = "UI.WindowMaximized";
    public const string GridLineColor = "UI.GridLineColor";
    public const string BridgeColor = "UI.BridgeColor";
    public const string IslandColor = "UI.IslandColor";
    public const string AnimationsEnabled = "UI.AnimationsEnabled";
}
```

#### Performance Settings
```csharp
public class PerformanceSettings
{
    public const string MaxGenerationTime = "Performance.MaxGenerationTime";
    public const string MaxSolverTime = "Performance.MaxSolverTime";
    public const string ParallelProcessing = "Performance.ParallelProcessing";
    public const string CacheSize = "Performance.CacheSize";
    public const string LogLevel = "Performance.LogLevel";
}
```

## Default Settings Configuration

### Embedded Settings File
The application includes a default settings file embedded as a resource:

**Location**: `Gui/Hashi.Gui/Resources/HashiSettings.json`

```json
{
  "Game": {
    "DefaultDifficulty": 3,
    "DefaultGridWidth": 15,
    "DefaultGridHeight": 15,
    "ShowHints": true,
    "AutoSave": true,
    "TimerEnabled": true,
    "SoundEnabled": false
  },
  "UI": {
    "Theme": "Light",
    "Language": "en-US",
    "WindowWidth": 1200,
    "WindowHeight": 800,
    "WindowMaximized": false,
    "GridLineColor": "#CCCCCC",
    "BridgeColor": "#0066CC",
    "IslandColor": "#FF6600",
    "AnimationsEnabled": true
  },
  "Performance": {
    "MaxGenerationTime": 30000,
    "MaxSolverTime": 10000,
    "ParallelProcessing": true,
    "CacheSize": 100,
    "LogLevel": "Info"
  }
}
```

### Settings Initialization
```csharp
public class ResourceManager : IResourceManager
{
    private readonly ISettingsProvider settingsProvider;
    private readonly ILogger logger;

    public ResourceManager(ISettingsProvider settingsProvider, ILogger logger)
    {
        this.settingsProvider = settingsProvider;
        this.logger = logger;
    }

    public void PrepareUi()
    {
        try
        {
            // Load settings from file if exists, otherwise use defaults
            if (File.Exists(GetSettingsPath()))
            {
                settingsProvider.LoadSettings();
                logger.Info("Settings loaded from file");
            }
            else
            {
                ResetSettingsAndLoadFromDefault();
                logger.Info("Default settings loaded");
            }
        }
        catch (Exception ex)
        {
            logger.Error("Failed to load settings, using defaults", ex);
            ResetSettingsAndLoadFromDefault();
        }
    }

    public void ResetSettingsAndLoadFromDefault()
    {
        // Load embedded default settings
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Hashi.Gui.Resources.HashiSettings.json";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        var defaultSettings = reader.ReadToEnd();
        
        var settings = JsonConvert.DeserializeObject<JObject>(defaultSettings);
        LoadSettingsFromJson(settings);
        
        settingsProvider.SaveSettings();
    }
}
```

## Settings Persistence

### User Settings Location
Settings are stored in the user's application data directory:

**Windows**: `%APPDATA%\Hashiwokakero\settings.json`
**Format**: JSON with hierarchical structure

### Settings File Structure
```json
{
  "Game": {
    "DefaultDifficulty": 4,
    "DefaultGridWidth": 12,
    "DefaultGridHeight": 12,
    "ShowHints": false,
    "AutoSave": true,
    "TimerEnabled": true,
    "SoundEnabled": true
  },
  "UI": {
    "Theme": "Dark",
    "Language": "de-DE",
    "WindowWidth": 1400,
    "WindowHeight": 900,
    "WindowLeft": 100,
    "WindowTop": 50,
    "WindowMaximized": true,
    "GridLineColor": "#444444",
    "BridgeColor": "#66AAFF",
    "IslandColor": "#FFAA66",
    "AnimationsEnabled": false
  },
  "Performance": {
    "MaxGenerationTime": 60000,
    "MaxSolverTime": 15000,
    "ParallelProcessing": true,
    "CacheSize": 200,
    "LogLevel": "Debug"
  },
  "LastModified": "2024-01-15T10:30:00Z",
  "Version": "1.0.0"
}
```

### Auto-Save Functionality
```csharp
public class SettingsProvider : ISettingsProvider
{
    private readonly Dictionary<string, object> settings = new();
    private readonly Timer autoSaveTimer;
    private bool hasChanges = false;

    public SettingsProvider()
    {
        // Auto-save every 30 seconds if there are changes
        autoSaveTimer = new Timer(30000);
        autoSaveTimer.Elapsed += OnAutoSaveTimer;
        autoSaveTimer.Start();
    }

    public void SetSetting<T>(string key, T value)
    {
        settings[key] = value;
        hasChanges = true;
        
        // Immediate save for critical settings
        if (IsCriticalSetting(key))
        {
            SaveSettings();
        }
    }

    private void OnAutoSaveTimer(object sender, ElapsedEventArgs e)
    {
        if (hasChanges)
        {
            SaveSettings();
        }
    }

    private bool IsCriticalSetting(string key)
    {
        return key.StartsWith("UI.Window") || 
               key.Equals(UISettings.Language) ||
               key.Equals(UISettings.Theme);
    }
}
```

## Internationalization (i18n)

### Language Support Architecture
The application supports multiple languages through a comprehensive translation system:

1. **Language Resources**: RESX files for each supported language
2. **Translation Service**: Dynamic language switching
3. **Culture Management**: Proper culture handling for formatting

### Supported Languages
- **English (en-US)**: Default language
- **German (de-DE)**: Fully supported
- **French (fr-FR)**: Fully supported
- **Spanish (es-ES)**: Fully supported
- **Japanese (ja-JP)**: Partially supported

### Translation Architecture

#### Language Resource Files
```
Gui/Hashi.Gui.Language/
├── Resources/
│   ├── Strings.resx          # Default (English)
│   ├── Strings.de-DE.resx    # German
│   ├── Strings.fr-FR.resx    # French
│   ├── Strings.es-ES.resx    # Spanish
│   └── Strings.ja-JP.resx    # Japanese
```

#### Translation Service
```csharp
public interface ITranslationService
{
    string GetString(string key);
    string GetString(string key, params object[] args);
    void SetLanguage(string cultureCode);
    string CurrentLanguage { get; }
    IEnumerable<string> AvailableLanguages { get; }
}

public class TranslationService : ITranslationService
{
    private readonly ResourceManager resourceManager;
    private CultureInfo currentCulture;

    public TranslationService()
    {
        resourceManager = new ResourceManager("Hashi.Gui.Language.Resources.Strings", 
            typeof(TranslationService).Assembly);
        currentCulture = CultureInfo.CurrentUICulture;
    }

    public string GetString(string key)
    {
        var value = resourceManager.GetString(key, currentCulture);
        return value ?? $"[{key}]"; // Return key in brackets if not found
    }

    public void SetLanguage(string cultureCode)
    {
        currentCulture = new CultureInfo(cultureCode);
        Thread.CurrentThread.CurrentUICulture = currentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
        
        // Notify UI to refresh
        LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(cultureCode));
    }

    public event EventHandler<LanguageChangedEventArgs> LanguageChanged;
}
```

### XAML Localization

#### Resource Binding
```xml
<Window x:Class="Hashi.Gui.Views.MainWindow"
        xmlns:resx="clr-namespace:Hashi.Gui.Language.Resources"
        Title="{x:Static resx:Strings.AppTitle}">
    
    <Grid>
        <Menu>
            <MenuItem Header="{x:Static resx:Strings.MenuFile}">
                <MenuItem Header="{x:Static resx:Strings.MenuFileNew}" 
                          Command="{Binding NewGameCommand}" />
                <MenuItem Header="{x:Static resx:Strings.MenuFileOpen}" 
                          Command="{Binding OpenGameCommand}" />
            </MenuItem>
            <MenuItem Header="{x:Static resx:Strings.MenuGame}">
                <MenuItem Header="{x:Static resx:Strings.MenuGameHint}" 
                          Command="{Binding HintCommand}" />
                <MenuItem Header="{x:Static resx:Strings.MenuGameSolve}" 
                          Command="{Binding SolveCommand}" />
            </MenuItem>
        </Menu>
        
        <StatusBar>
            <StatusBarItem Content="{x:Static resx:Strings.StatusReady}" />
        </StatusBar>
    </Grid>
</Window>
```

#### Dynamic Language Switching
```csharp
public class LanguageManager
{
    private readonly ITranslationService translationService;
    private readonly ISettingsProvider settingsProvider;

    public LanguageManager(ITranslationService translationService, ISettingsProvider settingsProvider)
    {
        this.translationService = translationService;
        this.settingsProvider = settingsProvider;
    }

    public void ChangeLanguage(string cultureCode)
    {
        translationService.SetLanguage(cultureCode);
        settingsProvider.SetSetting(UISettings.Language, cultureCode);
        
        // Update all UI elements
        UpdateUILanguage();
    }

    private void UpdateUILanguage()
    {
        // Force UI to refresh with new language
        Application.Current.Dispatcher.Invoke(() =>
        {
            var windows = Application.Current.Windows;
            foreach (Window window in windows)
            {
                // Refresh bindings
                BindingOperations.GetBindingExpression(window, Window.TitleProperty)?.UpdateTarget();
                RefreshControlBindings(window);
            }
        });
    }
}
```

### String Resources

#### Common Strings
```xml
<!-- Strings.resx -->
<data name="AppTitle" xml:space="preserve">
  <value>Hashiwokakero</value>
</data>
<data name="MenuFile" xml:space="preserve">
  <value>File</value>
</data>
<data name="MenuFileNew" xml:space="preserve">
  <value>New Game</value>
</data>
<data name="MenuFileOpen" xml:space="preserve">
  <value>Open Game</value>
</data>
<data name="MenuFileSave" xml:space="preserve">
  <value>Save Game</value>
</data>
<data name="MenuGame" xml:space="preserve">
  <value>Game</value>
</data>
<data name="MenuGameHint" xml:space="preserve">
  <value>Hint</value>
</data>
<data name="MenuGameSolve" xml:space="preserve">
  <value>Solve</value>
</data>
```

#### German Translation
```xml
<!-- Strings.de-DE.resx -->
<data name="AppTitle" xml:space="preserve">
  <value>Hashiwokakero</value>
</data>
<data name="MenuFile" xml:space="preserve">
  <value>Datei</value>
</data>
<data name="MenuFileNew" xml:space="preserve">
  <value>Neues Spiel</value>
</data>
<data name="MenuFileOpen" xml:space="preserve">
  <value>Spiel öffnen</value>
</data>
<data name="MenuFileSave" xml:space="preserve">
  <value>Spiel speichern</value>
</data>
<data name="MenuGame" xml:space="preserve">
  <value>Spiel</value>
</data>
<data name="MenuGameHint" xml:space="preserve">
  <value>Hinweis</value>
</data>
<data name="MenuGameSolve" xml:space="preserve">
  <value>Lösen</value>
</data>
```

## Theme System

### Theme Configuration
The application supports multiple visual themes:

#### Available Themes
- **Light**: Default light theme
- **Dark**: Dark theme for reduced eye strain
- **High Contrast**: Accessibility theme
- **Custom**: User-defined themes

#### Theme Settings
```csharp
public class ThemeSettings
{
    public const string PrimaryColor = "Theme.PrimaryColor";
    public const string SecondaryColor = "Theme.SecondaryColor";
    public const string BackgroundColor = "Theme.BackgroundColor";
    public const string ForegroundColor = "Theme.ForegroundColor";
    public const string AccentColor = "Theme.AccentColor";
    public const string GridLineColor = "Theme.GridLineColor";
    public const string BridgeColor = "Theme.BridgeColor";
    public const string IslandColor = "Theme.IslandColor";
    public const string HighlightColor = "Theme.HighlightColor";
}
```

#### Theme Application
```csharp
public class ThemeManager
{
    private readonly ISettingsProvider settingsProvider;
    private readonly ResourceDictionary themeResources;

    public ThemeManager(ISettingsProvider settingsProvider)
    {
        this.settingsProvider = settingsProvider;
        themeResources = new ResourceDictionary();
    }

    public void ApplyTheme(string themeName)
    {
        var themeUri = new Uri($"pack://application:,,,/Hashi.Gui;component/Resources/Themes/{themeName}.xaml");
        
        // Load theme resource dictionary
        var themeDict = new ResourceDictionary { Source = themeUri };
        
        // Apply to application resources
        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(themeDict);
        
        // Save theme preference
        settingsProvider.SetSetting(UISettings.Theme, themeName);
    }
}
```

## Logging Configuration

### NLog Configuration
The application uses NLog for comprehensive logging with configurable output targets and levels.

#### NLog.config
```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      internalLogLevel="Info"
      internalLogFile="c:\temp\internal-nlog.txt">

  <!-- Define targets -->
  <targets>
    <!-- File target -->
    <target xsi:type="File"
            name="fileTarget"
            fileName="logs/hashi-${shortdate}.log"
            layout="${longdate} ${uppercase:${level}} ${logger} ${message} ${exception:format=tostring}"
            archiveFileName="logs/archive/hashi-{#}.log"
            archiveEvery="Day"
            archiveNumbering="Rolling"
            maxArchiveFiles="7"
            concurrentWrites="true"
            keepFileOpen="false" />

    <!-- Console target -->
    <target xsi:type="Console"
            name="consoleTarget"
            layout="${time} ${uppercase:${level}} ${logger:shortName=true} ${message} ${exception:format=tostring}" />

    <!-- Debug output target -->
    <target xsi:type="Debugger"
            name="debugTarget"
            layout="${time} ${uppercase:${level}} ${logger:shortName=true} ${message}" />
  </targets>

  <!-- Define rules -->
  <rules>
    <!-- All logs to file -->
    <logger name="*" minlevel="Info" writeTo="fileTarget" />
    
    <!-- Debug and higher to console (only in debug builds) -->
    <logger name="*" minlevel="Debug" writeTo="consoleTarget" 
            condition="'${configuration}' == 'Debug'" />
    
    <!-- Debug output in Visual Studio -->
    <logger name="*" minlevel="Debug" writeTo="debugTarget" 
            condition="'${configuration}' == 'Debug'" />
  </rules>
</nlog>
```

### Dynamic Log Level Configuration
```csharp
public class LoggingConfiguration
{
    public static void ConfigureLogging(ISettingsProvider settingsProvider)
    {
        var logLevel = settingsProvider.GetSetting(PerformanceSettings.LogLevel, "Info");
        
        // Update NLog configuration
        var config = LogManager.Configuration;
        var rule = config.LoggingRules.FirstOrDefault();
        
        if (rule != null)
        {
            rule.EnableLoggingForLevel(LogLevel.FromString(logLevel));
            LogManager.Configuration = config;
        }
    }
}
```

## Advanced Configuration

### Environment-Specific Settings
```csharp
public class EnvironmentSettings
{
    public static void LoadEnvironmentSpecificSettings(ISettingsProvider settingsProvider)
    {
        var environment = Environment.GetEnvironmentVariable("HASHI_ENVIRONMENT") ?? "Production";
        
        switch (environment.ToLowerInvariant())
        {
            case "development":
                settingsProvider.SetSetting(PerformanceSettings.LogLevel, "Debug");
                settingsProvider.SetSetting(PerformanceSettings.MaxGenerationTime, 60000);
                break;
                
            case "testing":
                settingsProvider.SetSetting(PerformanceSettings.LogLevel, "Info");
                settingsProvider.SetSetting(PerformanceSettings.ParallelProcessing, false);
                break;
                
            case "production":
                settingsProvider.SetSetting(PerformanceSettings.LogLevel, "Warn");
                settingsProvider.SetSetting(PerformanceSettings.MaxGenerationTime, 30000);
                break;
        }
    }
}
```

### Configuration Validation
```csharp
public class ConfigurationValidator
{
    public static ValidationResult ValidateSettings(ISettingsProvider settingsProvider)
    {
        var result = new ValidationResult();
        
        // Validate game settings
        var difficulty = settingsProvider.GetSetting(GameSettings.DefaultDifficulty, DifficultyEnum.Medium1);
        if (!Enum.IsDefined(typeof(DifficultyEnum), difficulty))
        {
            result.AddError("Invalid default difficulty setting");
        }
        
        // Validate UI settings
        var windowWidth = settingsProvider.GetSetting(UISettings.WindowWidth, 1200);
        if (windowWidth < 800 || windowWidth > 4000)
        {
            result.AddError("Window width must be between 800 and 4000 pixels");
        }
        
        // Validate performance settings
        var maxGenTime = settingsProvider.GetSetting(PerformanceSettings.MaxGenerationTime, 30000);
        if (maxGenTime < 1000 || maxGenTime > 300000)
        {
            result.AddError("Max generation time must be between 1 and 300 seconds");
        }
        
        return result;
    }
}
```

## Settings Migration

### Version Compatibility
```csharp
public class SettingsMigrator
{
    public static void MigrateSettings(ISettingsProvider settingsProvider)
    {
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var settingsVersion = settingsProvider.GetSetting("Version", new Version(1, 0, 0));
        
        if (settingsVersion < currentVersion)
        {
            PerformMigration(settingsProvider, settingsVersion, currentVersion);
            settingsProvider.SetSetting("Version", currentVersion.ToString());
        }
    }
    
    private static void PerformMigration(ISettingsProvider settingsProvider, 
        Version fromVersion, Version toVersion)
    {
        // Example migration from 1.0 to 1.1
        if (fromVersion < new Version(1, 1, 0))
        {
            // Migrate old setting names
            var oldDifficulty = settingsProvider.GetSetting("DefaultDifficulty", DifficultyEnum.Medium1);
            settingsProvider.SetSetting(GameSettings.DefaultDifficulty, oldDifficulty);
            settingsProvider.RemoveSetting("DefaultDifficulty");
        }
    }
}
```

---

*For user gameplay guidance, see [User Guide](User-Guide.md)*