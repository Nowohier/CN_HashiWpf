namespace Hashi.LinearSolver.Interfaces.Models
{
    /// <summary>
    /// Represents an island in the Hashiwokakero puzzle.
    /// </summary>
    public interface IIsland
    {
        /// <summary>
        /// Gets the unique identifier for the island.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the row index of the island in the puzzle grid.
        /// </summary>
        int Row { get; }

        /// <summary>
        /// Gets the column index of the island in the puzzle grid.
        /// </summary>
        int Col { get; }

        /// <summary>
        /// Gets the number of bridges required to connect this island to others.
        /// </summary>
        int BridgesRequired { get; }

        /// <summary>
        /// Gets the list of neighboring islands' IDs that this island is connected to.
        /// </summary>
        List<int> Neighbors { get; }
    }
}
