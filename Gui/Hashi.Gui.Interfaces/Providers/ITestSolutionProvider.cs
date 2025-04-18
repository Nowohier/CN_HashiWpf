using Hashi.Generator.Interfaces.Providers;

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
    ///     Saves the test fields to a JSON file.
    /// </summary>
    void SaveTestFields();
}