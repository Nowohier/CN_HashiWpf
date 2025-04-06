using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.ViewModels;

namespace Hashi.Gui.Messages
{
    /// <summary>
    /// Represents the message for setting the potential target island.
    /// </summary>
    public class PotentialTargetIslandChangedMessage : ValueChangedMessage<IslandViewModel?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PotentialTargetIslandChangedMessage"/> class.
        /// </summary>
        /// <param name="target">The potential target island.</param>
        public PotentialTargetIslandChangedMessage(IslandViewModel? target) : base(target)
        {
        }
    }
}
