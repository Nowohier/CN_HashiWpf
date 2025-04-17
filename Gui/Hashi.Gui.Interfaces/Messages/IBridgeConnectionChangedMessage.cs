using Hashi.Gui.Interfaces.Messages.MessageContainers;

namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents the message for the bridge connection changed.
/// </summary>
public interface IBridgeConnectionChangedMessage : IValueChangedMessage<IBridgeConnectionInformationContainer>;