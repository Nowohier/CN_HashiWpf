using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages;

/// <inheritdoc cref="IIsTestModeRequestMessage" />
public class IsTestModeRequestMessage : RequestMessage<bool>, IIsTestModeRequestMessage;