using CommunityToolkit.Mvvm.Messaging.Messages;
using Hashi.Enums;
using Hashi.Gui.Interfaces.Messages;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Messages
{
    /// <inheritdoc cref="IDragDirectionChangedRequestTargetMessage"/>
    public class DragDirectionChangedRequestTargetMessage(IIslandViewModel source, DirectionEnum direction)
        : RequestMessage<IIslandViewModel?>, IDragDirectionChangedRequestTargetMessage
    {
        /// <inheritdoc />
        public IIslandViewModel Source { get; } = source;

        /// <inheritdoc />
        public DirectionEnum Direction { get; } = direction;
    }
}
