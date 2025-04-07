using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages;

/// <inheritdoc cref="IAllConnectionsSetMessage" />
public class AllConnectionsSetMessage : AsyncMessage<bool>, IAllConnectionsSetMessage
{
    public AllConnectionsSetMessage(bool value = true) : base(value)
    {
    }
}