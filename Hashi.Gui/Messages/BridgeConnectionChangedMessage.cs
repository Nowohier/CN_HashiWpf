using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IBridgeConnectionChangedMessage"/>
    public class BridgeConnectionChangedMessage : ValueChangedMessage<IBridgeConnectionInformationContainer>, IBridgeConnectionChangedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeConnectionChangedMessage"/> class.
        /// </summary>
        /// <param name="islandInfos">The information about the islands to be connected.</param>
        public BridgeConnectionChangedMessage(IBridgeConnectionInformationContainer islandInfos) : base(islandInfos)
        {
        }
    }
}
