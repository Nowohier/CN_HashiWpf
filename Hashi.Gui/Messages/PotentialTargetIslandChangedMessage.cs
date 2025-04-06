using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IPotentialTargetIslandChangedMessage"/>
    public class PotentialTargetIslandChangedMessage : ValueChangedMessage<IIslandViewModel?>, IPotentialTargetIslandChangedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PotentialTargetIslandChangedMessage"/> class.
        /// </summary>
        /// <param name="target">The potential target island.</param>
        public PotentialTargetIslandChangedMessage(IIslandViewModel? target) : base(target)
        {
        }
    }
}
