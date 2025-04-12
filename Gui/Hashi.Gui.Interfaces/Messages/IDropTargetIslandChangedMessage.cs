using Hashi.Gui.Interfaces.Messages.MessageContainers;

namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents the message for setting the drop target island.
/// </summary>
public interface IDropTargetIslandChangedMessage : IValueChangedMessage<IBridgeConnectionInformationContainer?>
{
}