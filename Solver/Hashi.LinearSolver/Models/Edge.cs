using Hashi.LinearSolver.Interfaces.Models;

namespace Hashi.LinearSolver.Models
{
    /// <summary>
    /// Represents an edge in the Hashi puzzle, connecting two islands.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="a">The ID of the first island in the edge.</param>
    /// <param name="b">The ID of the second island in the edge.</param>
    public class Edge(int id, int a, int b) : IEdge
    {
        /// <inheritdoc />
        public int Id { get; } = id;

        /// <inheritdoc />
        public int IslandA { get; } = a;

        /// <inheritdoc />
        public int IslandB { get; } = b;
    }
}
