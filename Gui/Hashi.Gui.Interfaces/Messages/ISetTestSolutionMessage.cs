using Hashi.Generator.Interfaces.Providers;

namespace Hashi.Gui.Interfaces.Messages
{
    /// <summary>
    ///   Represents the message sent when the test solution is set.
    /// </summary>
    public interface ISetTestSolutionMessage : IValueChangedMessage<ISolutionProvider>;
}
