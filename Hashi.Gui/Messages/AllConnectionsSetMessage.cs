using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Hashi.Gui.Messages
{
    public class AllConnectionsSetMessage : ValueChangedMessage<bool>
    {
        public AllConnectionsSetMessage(bool value = true) : base(value)
        {
        }
    }
}
