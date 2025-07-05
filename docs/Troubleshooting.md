# Troubleshooting

This guide helps you resolve common issues with the Hashiwokakero WPF application.

## Installation and Setup Issues

### .NET Framework Issues

#### Error: "The application requires .NET 8.0"
**Symptoms**: Application won't start, shows .NET version error

**Solutions**:
1. **Download .NET 8.0 Runtime**: 
   - Visit [Microsoft .NET Download Page](https://dotnet.microsoft.com/download/dotnet/8.0)
   - Download ".NET Desktop Runtime 8.0.x"
   - Install and restart your computer

2. **Verify Installation**:
   ```bash
   dotnet --version
   ```
   Should show version 8.0.x or later

3. **Check Registry** (Advanced):
   - Open Registry Editor
   - Navigate to `HKEY_LOCAL_MACHINE\SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost`
   - Verify .NET 8.0 entries exist

#### Error: "Could not load file or assembly"
**Symptoms**: Runtime errors about missing assemblies

**Solutions**:
1. **Repair .NET Installation**:
   - Download .NET 8.0 installer again
   - Run with `/repair` flag: `dotnet-runtime-8.0.x-win-x64.exe /repair`

2. **Clear NuGet Cache**:
   ```bash
   dotnet nuget locals all --clear
   ```

3. **Rebuild Application**:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

### Windows Desktop Issues

#### Error: "Microsoft.WindowsDesktop.App not found"
**Symptoms**: WPF application won't start

**Solutions**:
1. **Install Windows Desktop Runtime**:
   - Download "Windows Desktop Runtime 8.0.x" from Microsoft
   - This is separate from the regular .NET runtime

2. **Verify Windows Version**:
   - Windows 10 version 1903 or later required
   - Windows 11 fully supported

#### Error: "UseWPF requires Windows"
**Symptoms**: Build error on non-Windows systems

**Solutions**:
1. **Use Windows for WPF Development**:
   - WPF only runs on Windows
   - Use Windows VM or dual-boot for development

2. **Alternative for Other Platforms**:
   - The core libraries (Generator, Solver, Rules) work cross-platform
   - Only the GUI project requires Windows

## Build and Compilation Issues

### Visual Studio Build Errors

#### Error: "Project file is incomplete or corrupted"
**Symptoms**: Solution won't load or build

**Solutions**:
1. **Close Visual Studio**
2. **Delete bin and obj folders**:
   ```bash
   # In project root
   rmdir /s bin
   rmdir /s obj
   ```
3. **Clear Visual Studio cache**:
   - Delete `%localappdata%\Microsoft\VisualStudio\17.0_xxx\ComponentModelCache`
4. **Restart Visual Studio**
5. **Clean and rebuild**

#### Error: "Unable to resolve service for type"
**Symptoms**: Dependency injection errors at runtime

**Solutions**:
1. **Check AutoFac Modules**:
   ```csharp
   // Ensure all modules are registered
   builder.RegisterModule<AutoFacGeneratorModule>();
   builder.RegisterModule<AutoFacLinearSolverModule>();
   // ... etc
   ```

2. **Verify Interface Implementations**:
   - Check that concrete classes implement required interfaces
   - Ensure interfaces are properly registered

3. **Check Circular Dependencies**:
   - Review constructor dependencies
   - Break circular references with factory patterns

### NuGet Package Issues

#### Error: "Package restore failed"
**Symptoms**: Missing references, build errors about packages

**Solutions**:
1. **Clear NuGet Cache**:
   ```bash
   dotnet nuget locals all --clear
   ```

2. **Delete packages folder**:
   - Delete `packages` folder in solution root
   - Restore packages: `dotnet restore`

3. **Check Package Sources**:
   ```bash
   dotnet nuget list source
   ```
   Ensure nuget.org is enabled

4. **Manually Restore**:
   ```bash
   dotnet restore --force
   ```

#### Error: "Package version conflicts"
**Symptoms**: Multiple versions of same package

**Solutions**:
1. **Use Directory.Build.props**:
   ```xml
   <Project>
     <PropertyGroup>
       <AutofacVersion>8.2.1</AutofacVersion>
       <NRulesVersion>1.0.2</NRulesVersion>
     </PropertyGroup>
   </Project>
   ```

2. **Update All Projects**:
   ```bash
   dotnet add package Autofac --version 8.2.1
   ```

## Runtime Issues

### Application Startup Problems

#### Error: "Application failed to start"
**Symptoms**: Application crashes immediately on startup

**Solutions**:
1. **Check Event Viewer**:
   - Open Windows Event Viewer
   - Look in Windows Logs > Application
   - Find error entries for the application

2. **Enable Debug Logging**:
   ```xml
   <!-- In NLog.config -->
   <logger name="*" minlevel="Debug" writeTo="fileTarget" />
   ```

3. **Run from Command Line**:
   ```bash
   dotnet Hashi.Gui.dll
   ```
   Check console output for errors

4. **Check Dependencies**:
   - Verify all required DLLs are present
   - Use Dependency Walker or similar tool

#### Error: "Could not create SSL/TLS secure channel"
**Symptoms**: HTTPS/network operations fail

**Solutions**:
1. **Update Windows**:
   - Install latest Windows updates
   - Restart computer

2. **Check TLS Settings**:
   ```csharp
   // In App.xaml.cs
   ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
   ```

### Memory and Performance Issues

#### Problem: "Application runs slowly"
**Symptoms**: Sluggish UI, slow puzzle generation

**Solutions**:
1. **Check Available Memory**:
   - Close other applications
   - Ensure at least 2GB free RAM

2. **Disable Debug Logging**:
   ```xml
   <!-- In NLog.config for production -->
   <logger name="*" minlevel="Info" writeTo="fileTarget" />
   ```

3. **Reduce Grid Size**:
   - Use smaller puzzles (15x15 instead of 25x25)
   - Lower difficulty settings

4. **Disable Animations**:
   - Go to Settings > Display
   - Turn off animations

#### Problem: "Out of memory errors"
**Symptoms**: Application crashes with memory errors

**Solutions**:
1. **Increase Virtual Memory**:
   - Windows Settings > System > About > Advanced system settings
   - Performance Settings > Advanced > Virtual memory

2. **Check for Memory Leaks**:
   ```csharp
   // Ensure proper disposal
   public void Dispose()
   {
       timer?.Dispose();
       // Dispose other resources
   }
   ```

3. **Reduce Cache Size**:
   ```csharp
   settingsProvider.SetSetting("Performance.CacheSize", 50); // Reduce from default 100
   ```

## Puzzle Generation Issues

### Generation Failures

#### Error: "Failed to generate puzzle"
**Symptoms**: Puzzle generation times out or fails

**Solutions**:
1. **Reduce Complexity**:
   - Lower difficulty setting
   - Use fewer islands
   - Smaller grid size

2. **Increase Timeout**:
   ```csharp
   settingsProvider.SetSetting("Performance.MaxGenerationTime", 60000); // 60 seconds
   ```

3. **Check Resource Usage**:
   - Monitor CPU and memory during generation
   - Close other applications

#### Problem: "Generated puzzles are too easy/hard"
**Symptoms**: Difficulty doesn't match expectation

**Solutions**:
1. **Calibrate Difficulty**:
   - Try different difficulty levels
   - Adjust grid size accordingly

2. **Enable Difficulty Validation**:
   ```csharp
   var puzzle = await generator.GenerateHashAsync(20, 15, 15, 5, 0, true); // Last parameter = true
   ```

3. **Custom Parameters**:
   ```csharp
   var parameters = new GenerationParameters
   {
       Width = 12,
       Height = 12,
       NumberOfIslands = 25,
       Difficulty = DifficultyEnum.Medium2,
       CheckDifficulty = true
   };
   ```

### Solver Issues

#### Error: "Solver returned Infeasible"
**Symptoms**: Puzzle appears unsolvable

**Solutions**:
1. **Verify Puzzle Validity**:
   - Check if manually created puzzle is valid
   - Ensure all islands can be connected

2. **Check Constraints**:
   - Verify no islands are isolated
   - Ensure sufficient connection paths

3. **Solver Timeout**:
   ```csharp
   settingsProvider.SetSetting("Performance.MaxSolverTime", 30000); // 30 seconds
   ```

#### Problem: "Solver is too slow"
**Symptoms**: Solving takes very long time

**Solutions**:
1. **Enable Parallel Processing**:
   ```csharp
   settingsProvider.SetSetting("Performance.ParallelProcessing", true);
   ```

2. **Use Lazy Solving**:
   ```csharp
   var status = await solver.SolveLazy(grid); // Faster but less optimal
   ```

3. **Reduce Problem Size**:
   - Smaller grids solve faster
   - Fewer islands reduce complexity

## UI and Display Issues

### WPF Rendering Problems

#### Problem: "Blurry or scaled UI"
**Symptoms**: Text and graphics appear blurry

**Solutions**:
1. **Disable DPI Scaling**:
   - Right-click application executable
   - Properties > Compatibility
   - Check "Disable display scaling on high DPI settings"

2. **Application Manifest**:
   ```xml
   <application xmlns="urn:schemas-microsoft-com:asm.v3">
     <windowsSettings>
       <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
     </windowsSettings>
   </application>
   ```

3. **WPF DPI Settings**:
   ```csharp
   // In App.xaml.cs
   protected override void OnStartup(StartupEventArgs e)
   {
       SetProcessDPIAware();
       base.OnStartup(e);
   }

   [DllImport("user32.dll")]
   private static extern bool SetProcessDPIAware();
   ```

#### Problem: "UI elements not responding"
**Symptoms**: Buttons don't work, menus don't open

**Solutions**:
1. **Check for UI Thread Issues**:
   ```csharp
   // Ensure UI updates on correct thread
   Application.Current.Dispatcher.Invoke(() =>
   {
       // UI update code
   });
   ```

2. **Verify Command Bindings**:
   ```xml
   <Button Command="{Binding NewGameCommand}" 
           CommandParameter="{Binding SelectedDifficulty}" />
   ```

3. **Check DataContext**:
   - Verify ViewModels are properly set
   - Check binding expressions in XAML

### Theme and Appearance Issues

#### Problem: "Theme not applying correctly"
**Symptoms**: Colors or styles appear wrong

**Solutions**:
1. **Restart Application**:
   - Theme changes may require restart

2. **Clear Resource Cache**:
   ```csharp
   Application.Current.Resources.MergedDictionaries.Clear();
   // Reload theme resources
   ```

3. **Check Theme Files**:
   - Verify theme XAML files exist
   - Ensure proper resource keys

#### Problem: "High contrast mode issues"
**Symptoms**: Poor visibility in accessibility mode

**Solutions**:
1. **Test with High Contrast**:
   - Enable Windows High Contrast
   - Verify application still usable

2. **Provide Alternative Styles**:
   ```xml
   <Style x:Key="HighContrastButton" TargetType="Button">
     <Setter Property="Background" Value="{DynamicResource {x:Static SystemColors.ControlBrushKey}}" />
   </Style>
   ```

## Language and Localization Issues

### Translation Problems

#### Problem: "Text not translating"
**Symptoms**: Interface remains in default language

**Solutions**:
1. **Check Language Setting**:
   ```csharp
   var currentLang = settingsProvider.GetSetting("UI.Language", "en-US");
   translationService.SetLanguage(currentLang);
   ```

2. **Verify Resource Files**:
   - Check .resx files exist for target language
   - Ensure proper build action (Embedded Resource)

3. **Restart Application**:
   - Language changes may require restart

#### Problem: "Missing translations"
**Symptoms**: Some text shows as [Key] or English

**Solutions**:
1. **Add Missing Keys**:
   ```xml
   <!-- In Strings.de-DE.resx -->
   <data name="MissingKey" xml:space="preserve">
     <value>German Translation</value>
   </data>
   ```

2. **Fallback Handling**:
   ```csharp
   public string GetString(string key)
   {
       var value = resourceManager.GetString(key, currentCulture);
       return value ?? resourceManager.GetString(key, CultureInfo.InvariantCulture) ?? $"[{key}]";
   }
   ```

### Character Encoding Issues

#### Problem: "Special characters display incorrectly"
**Symptoms**: Umlauts, accents, or Unicode characters appear wrong

**Solutions**:
1. **Check File Encoding**:
   - Save .resx files as UTF-8
   - Verify proper BOM handling

2. **Font Support**:
   - Ensure fonts support required characters
   - Use fallback fonts for missing glyphs

3. **Culture Settings**:
   ```csharp
   Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
   Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
   ```

## File I/O and Data Issues

### Save/Load Problems

#### Error: "Access denied when saving"
**Symptoms**: Cannot save games or settings

**Solutions**:
1. **Check Permissions**:
   - Run application as administrator (temporary test)
   - Verify write permissions to application data folder

2. **Use User Data Directory**:
   ```csharp
   var userDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
   var appDataPath = Path.Combine(userDataPath, "Hashiwokakero");
   Directory.CreateDirectory(appDataPath);
   ```

3. **Handle Exceptions**:
   ```csharp
   try
   {
       await File.WriteAllTextAsync(filePath, content);
   }
   catch (UnauthorizedAccessException)
   {
       // Try alternative location or show error
   }
   ```

#### Problem: "Corrupted save files"
**Symptoms**: Cannot load previously saved games

**Solutions**:
1. **Validate File Format**:
   ```csharp
   try
   {
       var content = await File.ReadAllTextAsync(filePath);
       var data = JsonConvert.DeserializeObject<SaveData>(content);
       return ValidateSaveData(data);
   }
   catch (JsonException)
   {
       // Handle corrupted file
       return CreateBackupAndRetry(filePath);
   }
   ```

2. **Create Backups**:
   ```csharp
   // Before saving, create backup
   if (File.Exists(filePath))
   {
       File.Copy(filePath, $"{filePath}.backup", true);
   }
   ```

3. **Recovery Mode**:
   - Implement automatic recovery from backups
   - Provide manual file selection option

## Logging and Debugging

### Debug Information

#### Enabling Debug Logging
```xml
<!-- NLog.config for debugging -->
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd">
  <targets>
    <target xsi:type="File" name="debugFile"
            fileName="logs/debug-${shortdate}.log"
            layout="${longdate} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}" />
    <target xsi:type="Console" name="console"
            layout="${time} ${level:uppercase=true} ${logger:shortName=true} ${message}" />
  </targets>
  <rules>
    <logger name="*" minlevel="Debug" writeTo="debugFile,console" />
  </rules>
</nlog>
```

#### Getting System Information
```csharp
public static string GetSystemInfo()
{
    var info = new StringBuilder();
    info.AppendLine($"OS: {Environment.OSVersion}");
    info.AppendLine($".NET Version: {Environment.Version}");
    info.AppendLine($"Working Set: {Environment.WorkingSet / 1024 / 1024} MB");
    info.AppendLine($"Processor Count: {Environment.ProcessorCount}");
    info.AppendLine($"System Directory: {Environment.SystemDirectory}");
    info.AppendLine($"Current Directory: {Environment.CurrentDirectory}");
    return info.ToString();
}
```

### Performance Profiling

#### Memory Usage Monitoring
```csharp
public class MemoryMonitor
{
    private readonly Timer timer;
    private readonly ILogger logger;

    public MemoryMonitor(ILogger logger)
    {
        this.logger = logger;
        timer = new Timer(LogMemoryUsage, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    private void LogMemoryUsage(object? state)
    {
        var workingSet = Environment.WorkingSet / 1024 / 1024;
        var gcMemory = GC.GetTotalMemory(false) / 1024 / 1024;
        logger.Info($"Memory: Working Set = {workingSet} MB, GC = {gcMemory} MB");
    }
}
```

## Getting Help

### Information to Collect

When reporting issues, please provide:

1. **System Information**:
   - Windows version
   - .NET version (`dotnet --version`)
   - Application version

2. **Error Details**:
   - Exact error message
   - Steps to reproduce
   - Screenshots if applicable

3. **Log Files**:
   - Application logs from `logs/` folder
   - Windows Event Viewer entries

4. **Configuration**:
   - Settings file (`%APPDATA%\Hashiwokakero\settings.json`)
   - Any customizations made

### Support Channels

1. **GitHub Issues**:
   - Report bugs and feature requests
   - Search existing issues first

2. **Documentation**:
   - Check this wiki for solutions
   - Review API documentation

3. **Community**:
   - Discussion forums
   - Community contributions

### Creating Minimal Reproduction

When reporting bugs:

1. **Start with fresh installation**
2. **Use default settings**
3. **Document exact steps to reproduce**
4. **Note any error messages**
5. **Include system configuration**

Example bug report template:
```
**Description**: Brief description of the issue

**Steps to Reproduce**:
1. Start application
2. Click "New Game"
3. Select Expert difficulty
4. Click Generate

**Expected Behavior**: Puzzle should generate successfully

**Actual Behavior**: Application crashes with "Out of Memory" error

**Environment**:
- Windows 11 Pro (Build 22631)
- .NET 8.0.3
- Application Version 1.0.0
- 8GB RAM, Intel i5-8400

**Logs**: [Attach log files]
```

## Common Error Codes

### Application Error Codes

| Code | Description | Solution |
|------|-------------|----------|
| APP001 | Initialization failure | Check .NET installation |
| APP002 | Configuration error | Reset settings to default |
| APP003 | Resource loading error | Verify installation integrity |
| GEN001 | Puzzle generation timeout | Reduce complexity |
| GEN002 | Invalid generation parameters | Check parameter ranges |
| SOL001 | Solver initialization failed | Check OR-Tools installation |
| SOL002 | Solving timeout | Increase timeout setting |
| UI001 | WPF rendering error | Update graphics drivers |
| UI002 | Theme loading failed | Reset theme to default |
| IO001 | File access denied | Check permissions |
| IO002 | File corruption | Restore from backup |

---

*For additional help, see [Getting Started](Getting-Started.md) or [User Guide](User-Guide.md)*