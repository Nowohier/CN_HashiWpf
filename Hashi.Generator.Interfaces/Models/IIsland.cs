namespace Hashi.Generator.Interfaces.Models
{
    /// <summary>
    /// Interface for an island in the Hashi game.
    /// </summary>
    public interface IIsland
    {
        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the amount of bridges connectable.
        /// </summary>
        int AmountBridgesConnectable { get; set; }

        /// <summary>
        /// Gets the amount of bridges up.
        /// </summary>
        int AmountBridgesUp { get; set; }

        /// <summary>
        /// Gets the amount of bridges down.
        /// </summary>
        int AmountBridgesDown { get; set; }

        /// <summary>
        /// Gets the amount of bridges left.
        /// </summary>
        int AmountBridgesLeft { get; set; }

        /// <summary>
        /// Gets the amount of bridges right.
        /// </summary>
        int AmountBridgesRight { get; set; }

        /// <summary>
        /// Gets the upper neighbor.
        /// </summary>
        IIsland? IslandUp { get; }

        /// <summary>
        /// Gets the lower neighbor.
        /// </summary>
        IIsland? IslandDown { get; }

        /// <summary>
        /// Gets the left neighbor.
        /// </summary>
        IIsland? IslandLeft { get; }

        /// <summary>
        /// Gets the right neighbor.
        /// </summary>
        IIsland? IslandRight { get; }

        /// <summary>
        /// Set all neighbors of the island.
        /// </summary>
        /// <param name="field">The field array.</param>
        /// <param name="islands">The list of islands.</param>
        void SetAllNeighbors(int[][] field, List<IIsland> islands);
    }
}
