using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Generator.Interfaces.Providers;
using Hashi.Gui.Interfaces.Messages;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="ISetTestSolutionMessage"/>
    public class SetTestSolutionMessage(ISolutionProvider value)
        : ValueChangedMessage<ISolutionProvider>(value), ISetTestSolutionMessage;
}
