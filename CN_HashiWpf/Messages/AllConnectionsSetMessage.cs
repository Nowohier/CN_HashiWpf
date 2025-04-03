using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CNHashiWpf.Messages
{
    public class AllConnectionsSetMessage : ValueChangedMessage<bool>
    {
        public AllConnectionsSetMessage(bool value = true) : base(value)
        {
        }
    }
}
