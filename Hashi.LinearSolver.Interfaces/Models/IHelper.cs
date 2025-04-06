namespace Hashi.LinearSolver.Interfaces.Models
{
    /// <summary>
    /// Interface for helper classes that provide information about islands and bridges.
    /// </summary>
    public interface IHelper
    {
        /// <summary>
        /// Gets the list of islands.
        /// </summary>
        IList<int> Islands { get; }

        /// <summary>
        /// Gets the list of bridges.
        /// </summary>
        IList<int> Bridges { get; }
    }
}
