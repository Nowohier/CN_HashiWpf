using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.Messages.MessageContainers;

namespace Hashi.Gui.Messages;

/// <inheritdoc cref="IDropTargetIslandChangedMessage" />
public class DropTargetIslandChangedMessage : ValueChangedMessage<IBridgeConnectionInformationContainer>,
    IDropTargetIslandChangedMessage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DropTargetIslandChangedMessage" /> class.
    /// </summary>
    /// <param name="connectionInfo">The drop target connection info.</param>
    public DropTargetIslandChangedMessage(IBridgeConnectionInformationContainer connectionInfo) : base(connectionInfo)
    {
    }
}