using CNHashiWpf.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CNHashiWpf.Messages
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
