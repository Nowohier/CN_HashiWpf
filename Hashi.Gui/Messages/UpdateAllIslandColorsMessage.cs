using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IUpdateAllIslandColorsMessage"/>
    public class UpdateAllIslandColorsMessage : ValueChangedMessage<bool?>, IUpdateAllIslandColorsMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAllIslandColorsMessage"/> class.
        /// </summary>
        /// <param name="value">The color brush.</param>
        public UpdateAllIslandColorsMessage(bool? value = null) : base(value)
        {
        }
    }
}
