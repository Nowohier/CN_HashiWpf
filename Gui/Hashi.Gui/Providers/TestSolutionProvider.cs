using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Wrappers;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace Hashi.Gui.Providers;

[JsonObject(MemberSerialization.OptIn)]
public class TestSolutionProvider : ITestSolutionProvider
{
    private readonly IJsonWrapper jsonWrapper;
    private readonly IPathProvider pathProvider;

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