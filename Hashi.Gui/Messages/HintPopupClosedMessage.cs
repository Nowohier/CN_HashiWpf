using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IHintPopupClosedMessage" />
    public class HintPopupClosedMessage : ValueChangedMessage<bool?>, IHintPopupClosedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HintPopupClosedMessage"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public HintPopupClosedMessage(bool? value = null) : base(value)
        {
        }
    }
}
