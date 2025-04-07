using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Messages;

/// <summary>
///     Represents the message for setting the potential target island.
/// </summary>
public interface IPotentialTargetIslandChangedMessage : IValueChangedMessage<IIslandViewModel?>
{
}