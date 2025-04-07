using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Interfaces.Messages.MessageContainers;

public interface IBridgeConnectionInformationContainer
{
    /// <summary>
    ///     Gets the source island.
    /// </summary>
    IIslandViewModel SourceIsland { get; }

    /// <summary>
    ///     Gets the bridge operation type.
    /// </summary>
    BridgeOperationTypeEnum BridgeOperationType { get; }
}