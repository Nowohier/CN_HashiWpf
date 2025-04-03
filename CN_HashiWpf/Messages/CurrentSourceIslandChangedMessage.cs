using CNHashiWpf.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CNHashiWpf.Messages
{
    public class CurrentSourceIslandChangedMessage : ValueChangedMessage<IslandViewModel?>
    {
        public CurrentSourceIslandChangedMessage(IslandViewModel? value) : base(value)
        {
        }
    }
}
