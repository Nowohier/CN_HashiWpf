using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Messages
{
    public class CurrentSourceIslandChangedMessage : ValueChangedMessage<IslandViewModel?>
    {
        public CurrentSourceIslandChangedMessage(IslandViewModel? value) : base(value)
        {
        }
    }
}
