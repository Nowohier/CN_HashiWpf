using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Messages
{
    /// <summary>
    ///  Represents a request message to get the visible neighbors of an island.
    /// </summary>
    public interface IGetVisibleNeighborRequestMessage
    {
        /// <summary>
        /// The island for which the visible neighbors are requested.
        /// </summary>
        IIslandViewModel Source { get; }

        /// <summary>
        /// The potential target island .
        /// </summary>
        IIslandViewModel Target { get; }

        /// <summary>
        /// The response to the request message.
        /// </summary>
        IIslandViewModel? Response { get; }

        /// <summary>
        /// Replies to the request message with the given response.
        /// </summary>
        /// <param name="response">The response.</param>
        void Reply(IIslandViewModel? response);
    }
}
