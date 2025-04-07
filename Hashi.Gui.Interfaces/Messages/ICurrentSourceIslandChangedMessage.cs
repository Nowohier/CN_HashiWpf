using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents the message sent when the current source island changes.
/// </summary>
public interface ICurrentSourceIslandChangedMessage : IValueChangedMessage<IIslandViewModel?>
{
}