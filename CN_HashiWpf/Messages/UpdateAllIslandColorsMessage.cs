using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows.Media;

namespace CNHashiWpf.Messages
{
    public class UpdateAllIslandColorsMessage : ValueChangedMessage<Brush>
    {
        public UpdateAllIslandColorsMessage(Brush value) : base(value)
        {
        }
    }
}
