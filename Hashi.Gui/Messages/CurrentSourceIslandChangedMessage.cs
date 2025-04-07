using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Messages;

/// <inheritdoc cref="ICurrentSourceIslandChangedMessage" />
public class CurrentSourceIslandChangedMessage : ValueChangedMessage<IIslandViewModel?>,
    ICurrentSourceIslandChangedMessage
{
    public CurrentSourceIslandChangedMessage(IIslandViewModel? value) : base(value)
    {
    }
}