using CNHashiWpf.Enums;
using CNHashiWpf.ViewModels;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace CNHashiWpf.Messages.MessageContainers
{
    /// <summary>
    /// Represents the container for the bridge connection.
    /// </summary>
    public class BridgeConnectionInformationContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeConnectionInformationContainer"/> class.
        /// </summary>
        /// <param name="sourceIsland">The source island.</param>
        /// <param name="targetIsland">The target island.</param>
        /// <param name="bridgeOperationType">The bridge operation type.</param>
        public BridgeConnectionInformationContainer(IslandViewModel sourceIsland, IslandViewModel? targetIsland, BridgeOperationType bridgeOperationType)
        {
            SourceIsland = sourceIsland;
            TargetIsland = targetIsland;
            BridgeOperationType = bridgeOperationType;
        }

        /// <summary>
        /// Gets the source island.
        /// </summary>
        public IslandViewModel SourceIsland { get; }

        /// <summary>
        /// Gets the target island.
        /// </summary>
        public IslandViewModel? TargetIsland { get; }

        /// <summary>
        /// Gets the bridge operation type.
        /// </summary>
        public BridgeOperationType BridgeOperationType { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is diagonal and therefore not allowed.
        /// </summary>
        public bool IsDiagonalConnection => SourceIsland.Coordinates.X != TargetIsland.Coordinates.X && SourceIsland.Coordinates.Y != TargetIsland.Coordinates.Y;
    }
}
