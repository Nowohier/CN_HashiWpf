using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages;

/// <inheritdoc cref="IAllConnectionsSetMessage" />
public class AllConnectionsSetMessage(bool? value) : ValueChangedMessage<bool?>(value), IAllConnectionsSetMessage;