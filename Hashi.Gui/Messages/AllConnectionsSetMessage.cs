using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IAllConnectionsSetMessage"/>
    public class AllConnectionsSetMessage : ValueChangedMessage<bool>, IAllConnectionsSetMessage
    {
        public AllConnectionsSetMessage(bool value = true) : base(value)
        {
        }
    }
}
