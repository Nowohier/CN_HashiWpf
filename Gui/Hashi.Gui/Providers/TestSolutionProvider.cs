using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Logging.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Providers;

/// <inheritdoc cref="ITestSolutionProvider" />
[JsonObject(MemberSerialization.OptIn)]
public class TestSolutionProvider : ObservableObject, ITestSolutionProvider
{
    private readonly IJsonWrapper jsonWrapper;
    private readonly IPathProvider pathProvider;
    private readonly IFileWrapper fileWrapper;
    private readonly IDirectoryWrapper directoryWrapper;
    private readonly Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory;
    private readonly Func<ISolutionProvider, ISetTestSolutionMessage> setTestSolutionMessageFactory;
    private readonly ILogger logger;
    private ISolutionProvider? selectedSolutionProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TestSolutionProvider" /> class.
    /// </summary>
    /// <param name="jsonWrapper">The JSON serialization wrapper.</param>
    /// <param name="pathProvider">The path provider for file locations.</param>
    /// <param name="fileWrapper">The file system wrapper.</param>
    /// <param name="directoryWrapper">The directory wrapper.</param>
    /// <param name="solutionProviderFactory">The factory for creating solution providers.</param>
    /// <param name="setTestSolutionMessageFactory">The factory for creating test solution messages.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public TestSolutionProvider
    (
        IJsonWrapper jsonWrapper,
        IPathProvider pathProvider,
        IFileWrapper fileWrapper,
        IDirectoryWrapper directoryWrapper,
        Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory,
        Func<ISolutionProvider, ISetTestSolutionMessage> setTestSolutionMessageFactory,
        ILoggerFactory loggerFactory
    )
    {
        this.jsonWrapper = jsonWrapper ?? throw new ArgumentNullException(nameof(jsonWrapper));
        this.pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        this.fileWrapper = fileWrapper ?? throw new ArgumentNullException(nameof(fileWrapper));
        this.directoryWrapper = directoryWrapper ?? throw new ArgumentNullException(nameof(directoryWrapper));
        this.solutionProviderFactory = solutionProviderFactory ?? throw new ArgumentNullException(nameof(solutionProviderFactory));
        this.setTestSolutionMessageFactory = setTestSolutionMessageFactory ?? throw new ArgumentNullException(nameof(setTestSolutionMessageFactory));
        this.logger = loggerFactory?.CreateLogger<TestSolutionProvider>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        SolutionProviders.AddRange(LoadSettings());
        selectedSolutionProvider = SolutionProviders.FirstOrDefault();
        logger.Info("TestSolutionProvider initialized");
    }

    /// <inheritdoc />
    public IReadOnlyList<int[]> HashiFieldReference =>
    [
        [0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0],
        [0, 0, 0, 0, 0, 0]
    ];

    /// <inheritdoc />
    public ObservableCollection<ISolutionProvider> SolutionProviders { get; } = [];

    /// <inheritdoc />
    public ISolutionProvider? SelectedSolutionProvider
    {
        get => selectedSolutionProvider;
        set
        {
            if (!SetProperty(ref selectedSolutionProvider, value) || selectedSolutionProvider == null)
            {
                return;
            }

            var message = setTestSolutionMessageFactory.Invoke(selectedSolutionProvider);
            WeakReferenceMessenger.Default.Send(message);
        }
    }

    /// <inheritdoc />
    public void ResetSettings()
    {
        SolutionProviders.Clear();
        SolutionProviders.AddRange(LoadSettings());
    }

    /// <inheritdoc />
    public void ConvertIslandsToSolutionProvider(IEnumerable<IIslandViewModel> allIslandEnumerable)
    {
        ArgumentNullException.ThrowIfNull(allIslandEnumerable, nameof(allIslandEnumerable));
        ArgumentNullException.ThrowIfNull(SelectedSolutionProvider, nameof(SelectedSolutionProvider));

        var solutionName = SelectedSolutionProvider.Name;
        var solutionProvider = Converters.TestFieldConverter.ConvertIslandsToSolutionProvider(
            allIslandEnumerable, solutionName!, solutionProviderFactory);

        // Add the solution provider to the list
        if (SolutionProviders.Select(x => x.Name).Contains(solutionName))
        {
            var existingSolution = SolutionProviders.First(x => x.Name == solutionName);
            SolutionProviders.Remove(existingSolution);
        }

        SolutionProviders.Add(solutionProvider);
        SelectedSolutionProvider = solutionProvider;
    }

    /// <inheritdoc />
    public void SaveTestFields()
    {
        if (SolutionProviders == null)
        {
            throw new InvalidOperationException("Settings cannot be null.");
        }

        logger.Debug($"Saving test fields to {pathProvider.HashiTestFieldsFilePath}");
        var jsonArray = jsonWrapper.SerializeWithCustomIndenting(SolutionProviders.Where(x => x.Name != null));
        var path = pathProvider.HashiTestFieldsFilePath;
        try
        {
            if (!directoryWrapper.Exists(pathProvider.SettingsDirectoryPath))
            {
                logger.Debug($"Creating settings directory: {pathProvider.SettingsDirectoryPath}");
                directoryWrapper.CreateDirectory(pathProvider.SettingsDirectoryPath);
            }

            fileWrapper.WriteAllText(path, jsonArray);
            logger.Info($"Successfully saved {SolutionProviders.Count(x => x.Name != null)} test fields");
        }
        catch (Exception ex)
        {
            logger.Error("Failed to save test fields", ex);
        }
    }

    internal List<ISolutionProvider> LoadSettings()
    {
        var loadedTestFields = new List<ISolutionProvider>();

        try
        {
            logger.Debug($"Loading settings from {pathProvider.HashiTestFieldsFilePath}");
            var path = pathProvider.HashiTestFieldsFilePath;
            if (!fileWrapper.Exists(path) || fileWrapper.ReadAllText(path) is not { } fileContent ||
                jsonWrapper.DeserializeObject(fileContent, typeof(List<ISolutionProvider>)) is not
                    List<ISolutionProvider> loadedFromJson)
            {
                logger.Info("No existing test fields found, starting with empty list");
                return loadedTestFields;
            }

            loadedTestFields.AddRange(loadedFromJson);
            logger.Info($"Successfully loaded {loadedTestFields.Count} test fields");

            return loadedTestFields;
        }
        catch (Exception ex)
        {
            logger.Error("Failed to load settings", ex);
        }

        return loadedTestFields;
    }
}