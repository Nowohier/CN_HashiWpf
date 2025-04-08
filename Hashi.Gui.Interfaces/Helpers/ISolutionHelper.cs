using Hashi.Generator.Interfaces.Models;
using Hashi.Gui.Interfaces.ViewModels;
using System.Collections.ObjectModel;

namespace Hashi.Gui.Interfaces.Helpers
{
    /// <summary>
    ///  Provides helper methods for managing solutions in the Hashi game.
    /// </summary>
    public interface ISolutionHelper
    {
        /// <summary>
        ///   Generates a hint for the given solution container and islands.
        /// </summary>
        /// <param name="solutionContainer">The solution container.</param>
        /// <param name="islands">The islands.</param>
        void GenerateHint(ISolutionContainer solutionContainer, ObservableCollection<ObservableCollection<IIslandViewModel>> islands);

        /// <summary>
        ///     Gets all islands involved in a connection between two islands.
        /// </summary>
        /// <param name="source">The source island.</param>
        /// <param name="target">The target island.</param>
        /// <returns></returns>
        IEnumerable<IIslandViewModel> GetAllIslandsInvolvedInConnection(IIslandViewModel source, IIslandViewModel target, ObservableCollection<ObservableCollection<IIslandViewModel>> islands);
    }
}
