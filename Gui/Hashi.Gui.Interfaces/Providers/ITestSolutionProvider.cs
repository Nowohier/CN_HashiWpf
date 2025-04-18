using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Providers;

/// <summary>
///     Interface for providing test solutions.
/// </summary>
public interface ITestSolutionProvider
{
    /// <summary>
    ///     Gets the Hashi field reference.
    /// </summary>
    IReadOnlyList<int[]> HashiFieldReference { get; }

    /// <summary>
    ///     Gets the test solution providers.
    /// </summary>
    List<ISolutionProvider> SolutionProviders { get; }

    /// <summary>
    ///    Gets or sets the selected solution provider.
    /// </summary>
    ISolutionProvider? SelectedSolutionProvider { get; set; }

    /// <summary>
    ///     Saves the test fields to a JSON file.
    /// </summary>
    void SaveTestFields();

    /// <summary>
    /// Converts the current islands to a solution provider.
    /// </summary>
    /// <param name="allIslands">The islands as a flat IEnumerable.</param>
    void ConvertIslandsToSolutionProvider(IEnumerable<IIslandViewModel> allIslands);
}