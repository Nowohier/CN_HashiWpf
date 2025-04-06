namespace Hashi.LinearSolver.Interfaces.Models
{
    /// <summary>
    /// Represents a bridge between two islands.
    /// </summary>
    public interface IBridge
    {
        /// <summary>
        /// Gets the first island.
        /// </summary>
        IIsland Island1 { get; }

        /// <summary>
        /// Gets the second island.
        /// </summary>
        IIsland Island2 { get; }
    }
}
