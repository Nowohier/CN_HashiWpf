using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Providers;
using Hashi.Gui.Interfaces.Resources.SolutionProviders;
using System.Windows.Input;

namespace Hashi.Gui.Interfaces.ViewModels
{
    /// <summary>
    ///   The interface for the GenerateTestFieldViewModel.
    /// </summary>
    public interface IGenerateTestFieldViewModel
    {
        /// <summary>
        ///    Gets or sets the selected test solution provider.
        /// </summary>
        ISolutionProvider? SelectedTestSolutionProvider { get; set; }

        /// <summary>
        /// The island provider for the Hashi game.
        /// </summary>
        IIslandProvider IslandProvider { get; }

        /// <summary>
        ///    The hint provider for the Hashi game.
        /// </summary>
        IHintProvider HintProvider { get; }

        /// <summary>
        /// The test solution provider for the Hashi game.
        /// </summary>
        ITestSolutionProvider TestSolutionProvider { get; }

        /// <summary>
        ///     Removes all bridges from the game.
        /// </summary>
        ICommand RemoveAllBridgesCommand { get; }

        /// <summary>
        ///     Generates a hint for the game.
        /// </summary>
        ICommand GenerateHintCommand { get; }

        /// <summary>
        ///     Handles the mouse click event on the window.
        /// </summary>
        ICommand WindowMouseClickedCommand { get; }

        /// <summary>
        ///    Resets the test field to its initial state.
        /// </summary>
        /// <param name="solutionProvider">The solution provider.</param>
        void SetTestSolution(ISolutionProvider solutionProvider);

        /// <summary>
        ///     Handles the message when a bridge connection is changed.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentOutOfRangeException">The <see cref="IBridgeConnectionChangedMessage" />.</exception>
        void Receive(IBridgeConnectionChangedMessage message);

        /// <summary>
        ///     Updates the color of all islands.
        /// </summary>
        /// <param name="message">The <see cref="IUpdateAllIslandColorsMessage" />.</param>
        void Receive(IUpdateAllIslandColorsMessage message);

        /// <summary>
        ///     Handles the message when the potential target island is changed.
        /// </summary>
        /// <param name="islandChangedMessage">The <see cref="IDropTargetIslandChangedMessage" />.</param>
        void Receive(IDropTargetIslandChangedMessage islandChangedMessage);
    }
}
