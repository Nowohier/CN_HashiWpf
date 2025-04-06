using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Messages.MessageContainers;

namespace Hashi.Gui.Messages
{
    /// <summary>
    /// Represents the message for the bridge connection changed.
    /// </summary>
    public class BridgeConnectionChangedMessage : ValueChangedMessage<BridgeConnectionInformationContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeConnectionChangedMessage"/> class.
        /// </summary>
        /// <param name="islandInfos">The information about the islands to be connected.</param>
        public BridgeConnectionChangedMessage(BridgeConnectionInformationContainer islandInfos) : base(islandInfos)
        {
        }
    }
}
