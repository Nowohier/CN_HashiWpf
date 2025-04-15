using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages;

/// <inheritdoc cref="IRuleMessageClearedMessage" />
public class RuleMessageClearedMessage : ValueChangedMessage<bool?>, IRuleMessageClearedMessage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RuleMessageClearedMessage" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    public RuleMessageClearedMessage(bool? value = null) : base(value)
    {
    }
}