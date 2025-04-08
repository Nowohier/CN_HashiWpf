using Hashi.Gui.Enums;
using Hashi.Gui.Interfaces.Messages.MessageContainers;
using Hashi.Gui.Interfaces.ViewModels;

namespace Hashi.Gui.Messages.MessageContainers;

/// <inheritdoc cref="IBridgeConnectionInformationContainer"/>
public class BridgeConnectionInformationContainer : IBridgeConnectionInformationContainer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BridgeConnectionInformationContainer" /> class.
    /// </summary>
    /// <param name="bridgeOperationType">The bridge operation type.</param>
    /// <param name="sourceIsland">The source island.</param>
    /// <param name="targetIsland">The target island.</param>
    public BridgeConnectionInformationContainer(BridgeOperationTypeEnum bridgeOperationType, IIslandViewModel sourceIsland, IIslandViewModel? targetIsland = null)
    {
        BridgeOperationType = bridgeOperationType;
        SourceIsland = sourceIsland;
        TargetIsland = targetIsland;
    }

    /// <inheritdoc />
    public IIslandViewModel SourceIsland { get; }

    /// <inheritdoc />
    public IIslandViewModel? TargetIsland { get; }

    /// <inheritdoc />
    public BridgeOperationTypeEnum BridgeOperationType { get; }
}