using CommunityToolkit.Mvvm.ComponentModel;
using Hashi.Generator.Interfaces.Models;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Generator.Models;
using Hashi.Generator.Providers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;
using Hashi.Gui.Interfaces.Wrappers;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace Hashi.Gui.Providers;

[JsonObject(MemberSerialization.OptIn)]
public class TestSolutionProvider : ObservableObject, ITestSolutionProvider
{
    private readonly IJsonWrapper jsonWrapper;
    private readonly IPathProvider pathProvider;
    private ISolutionProvider? selectedSolutionProvider;

    public TestSolutionProvider(IJsonWrapper jsonWrapper, IPathProvider pathProvider)
    {
        this.jsonWrapper = jsonWrapper;
        this.pathProvider = pathProvider;
        SolutionProviders = LoadSettings(); //ToDo: Implement saving testfields!
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
    public List<ISolutionProvider> SolutionProviders { get; }

    /// <inheritdoc />
    public ISolutionProvider? SelectedSolutionProvider
    {
        get => selectedSolutionProvider;
        set
        {
            if (SetProperty(ref selectedSolutionProvider, value))
            {
                //ToDo : Implement this!
                //HintProvider.RuleInfoProvider.RuleMessage = TranslationSource.Instance[selectedRule.Name] ?? string.Empty;
                //HintProvider.RuleInfoProvider.AreRulesBeingApplied = false;

                //if (IsTestFieldMode) SetTestSolution(GetCurrentTestSolution());
            }
        }
    }

    /// <inheritdoc />
    public void ConvertIslandsToSolutionProvider(IEnumerable<IIslandViewModel> allIslandEnumerable, string solutionName)
    {
        ArgumentNullException.ThrowIfNull(allIslandEnumerable, nameof(allIslandEnumerable));

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

        // Create the solution provider
        var solutionProvider = new SolutionProvider(hashiField, bridgeCoordinates, solutionName);

        // Add the solution provider to the list
        if (SolutionProviders.Select(x => x.Name).Contains(solutionName))
        {
            var existingSolution = SolutionProviders.First(x => x.Name == solutionName);
            SolutionProviders.Remove(existingSolution);
        }

        SolutionProviders.Add(solutionProvider);
    }

    /// <inheritdoc />
    public void SaveTestFields()
    {
        if (SolutionProviders == null) throw new InvalidOperationException("Settings cannot be null.");

        var jsonArray = jsonWrapper.SerializeWithCustomIndenting(SolutionProviders);
        var path = pathProvider.HashiTestFieldsFilePath;
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

    private List<ISolutionProvider> LoadSettings()
    {
        var loadedTestFields = new List<ISolutionProvider>();

        try
        {
            var path = pathProvider.HashiTestFieldsFilePath;
            if (File.Exists(path))
            {
                loadedTestFields =
                    (List<ISolutionProvider>)jsonWrapper.DeserializeObject(File.ReadAllText(path),
                        typeof(List<ISolutionProvider>))!;
                return loadedTestFields;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.StackTrace);
        }

        return loadedTestFields;
    }
}