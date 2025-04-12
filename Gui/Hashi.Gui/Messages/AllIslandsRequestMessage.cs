using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Messages;

/// <summary>
///     Represents a request message to get all islands in the game.
/// </summary>
public class AllIslandsRequestMessage : RequestMessage<ObservableCollection<ObservableCollection<IIslandViewModel>>>,
    IAllIslandsRequestMessage
{
}