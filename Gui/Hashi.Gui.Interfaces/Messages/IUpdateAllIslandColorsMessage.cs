namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents the message sent when all island colors need to be updated.
/// </summary>
public interface IUpdateAllIslandColorsMessage : IValueChangedMessage<bool?>
{
}