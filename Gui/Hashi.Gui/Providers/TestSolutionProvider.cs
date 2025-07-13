using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Gui.Extensions;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Hashi.Logging.Interfaces;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;

namespace Hashi.Gui.Providers;

[JsonObject(MemberSerialization.OptIn)]
public class TestSolutionProvider : ObservableObject, ITestSolutionProvider
{
    private readonly IJsonWrapper jsonWrapper;
    private readonly IPathProvider pathProvider;
    private readonly Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory;
    private readonly Func<ISolutionProvider, ISetTestSolutionMessage> setTestSolutionMessageFactory;
    private readonly ILogger logger;
    private ISolutionProvider? selectedSolutionProvider;

    public TestSolutionProvider
    (
        IJsonWrapper jsonWrapper,
        IPathProvider pathProvider,
        Func<IReadOnlyList<int[]>?, List<IBridgeCoordinates>?, string?, ISolutionProvider> solutionProviderFactory,
        Func<ISolutionProvider, ISetTestSolutionMessage> setTestSolutionMessageFactory,
        ILoggerFactory loggerFactory
    )
    {
        this.jsonWrapper = jsonWrapper ?? throw new ArgumentNullException(nameof(jsonWrapper));
        this.pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
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
            if (!SetProperty(ref selectedSolutionProvider, value) || selectedSolutionProvider == null) return;
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

        var allIslands = allIslandEnumerable.ToList();

        // Determine the size of the Hashi field
        var maxX = allIslands.Max(island => island.Coordinates.X);
        var maxY = allIslands.Max(island => island.Coordinates.Y);
        var hashiField = new int[maxY + 1][];
        for (var i = 0; i <= maxY; i++)
        {
            hashiField[i] = new int[maxX + 1];
        }

        // Populate the Hashi field and map island coordinates
        foreach (var island in allIslands)
        {
            var x = island.Coordinates.X;
            var y = island.Coordinates.Y;
            hashiField[y][x] = island.MaxConnections;
        }

        // Create a list of bridge coordinates
        var bridgeCoordinates = new List<IBridgeCoordinates>();

        foreach (var island in allIslands.Where(x => x.MaxConnections > 0))
        {
            // Group connections by their target coordinates
            var groupedConnections = island.AllConnections
                .GroupBy(connection => new { connection.X, connection.Y })
                .Select(group => new
                {
                    Target = group.Key,
                    Amount = group.Count()
                });

            foreach (var connectionGroup in groupedConnections)
            {
                var bridge = new BridgeCoordinates(
                    new Point(island.Coordinates.X, island.Coordinates.Y),
                    new Point(connectionGroup.Target.X, connectionGroup.Target.Y),
                    connectionGroup.Amount
                );

                // Avoid duplicate bridges by checking if the reverse connection already exists
                if (!bridgeCoordinates.Any(b =>
                        (b.Location1 == bridge.Location1 && b.Location2 == bridge.Location2) ||
                        (b.Location1 == bridge.Location2 && b.Location2 == bridge.Location1)))
                {
                    bridgeCoordinates.Add(bridge);
                }
            }
        }

        bridgeCoordinates = bridgeCoordinates.Distinct().ToList();

        var solutionName = SelectedSolutionProvider.Name;

        // Create the solution provider
        var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, solutionName);

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
        if (SolutionProviders == null) throw new InvalidOperationException("Settings cannot be null.");

        logger.Debug($"Saving test fields to {pathProvider.HashiTestFieldsFilePath}");
        var jsonArray = jsonWrapper.SerializeWithCustomIndenting(SolutionProviders.Where(x => x.Name != null));
        var path = pathProvider.HashiTestFieldsFilePath;
        try
        {
            if (!Directory.Exists(pathProvider.SettingsDirectoryPath))
            {
                logger.Debug($"Creating settings directory: {pathProvider.SettingsDirectoryPath}");
                Directory.CreateDirectory(pathProvider.SettingsDirectoryPath);
            }

            File.WriteAllText(path, jsonArray);
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
            if (!File.Exists(path) || File.ReadAllText(path) is not { } fileContent ||
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