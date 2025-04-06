using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Hashi.Gui.Messages
{
    public class UpdateAllIslandColorsMessage : ValueChangedMessage<Brush>
    {
        public UpdateAllIslandColorsMessage(Brush value) : base(value)
        {
        }
    }
}
