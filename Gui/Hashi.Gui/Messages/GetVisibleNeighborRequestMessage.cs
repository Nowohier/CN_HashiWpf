using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IGetVisibleNeighborRequestMessage"/>
    public class GetVisibleNeighborRequestMessage(IIslandViewModel source, IIslandViewModel target) : RequestMessage<IIslandViewModel?>, IGetVisibleNeighborRequestMessage
    {
        /// <inheritdoc />
        public IIslandViewModel Source { get; } = source;

        /// <inheritdoc />
        public IIslandViewModel Target { get; } = target;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(GetVisibleNeighborRequestMessage)} | Source: {Source} | Target: {Target}";
        }
    }
}
