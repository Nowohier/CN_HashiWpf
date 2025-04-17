using Hashi.Enums;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Messages
{
    /// <summary>
    ///      The drag direction changed message.
    /// </summary>
    public interface IDragDirectionChangedRequestTargetMessage
    {
        /// <summary>
        /// The source island.
        /// </summary>
        public IIslandViewModel Source { get; }

        /// <summary>
        /// The direction of the drag.
        /// </summary>
        public DirectionEnum Direction { get; }

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
