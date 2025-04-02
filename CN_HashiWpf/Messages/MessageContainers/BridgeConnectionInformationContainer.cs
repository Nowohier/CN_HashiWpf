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
        /// <param name="receiverIsland">The receiver island.</param>
        public BridgeConnectionInformationContainer(IslandViewModel sourceIsland, IslandViewModel receiverIsland)
        {
            SourceIsland = sourceIsland;
            ReceiverIsland = receiverIsland;
        }

        /// <summary>
        /// Gets the source island.
        /// </summary>
        public IslandViewModel SourceIsland { get; }

        /// <summary>
        /// Gets the receiver island.
        /// </summary>
        public IslandViewModel ReceiverIsland { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is diagonal and therefore not allowed.
        /// </summary>
        public bool IsDiagonalConnection => SourceIsland.Coordinates.X != ReceiverIsland.Coordinates.X && SourceIsland.Coordinates.Y != ReceiverIsland.Coordinates.Y;
    }
}
