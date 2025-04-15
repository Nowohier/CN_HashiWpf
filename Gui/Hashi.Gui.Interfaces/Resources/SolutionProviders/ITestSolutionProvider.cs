using Hashi.Generator.Interfaces.Providers;

namespace Hashi.Gui.Interfaces.Resources.SolutionProviders
{
    /// <summary>
    ///   Interface for providing test solutions.
    /// </summary>
    public interface ITestSolutionProvider
    {
        /// <summary>
        ///   Gets the test solution providers.
        /// </summary>
        List<ISolutionProvider> SolutionProviders { get; }
    }
}
