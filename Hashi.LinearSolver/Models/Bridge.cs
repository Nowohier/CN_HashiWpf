namespace Hashi.LinearSolver.Models
{
    /// <summary>
    /// Represents a bridge between two islands.
    /// </summary>
    public class Bridge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bridge"/> class.
        /// </summary>
        /// <param name="island1">The first island.</param>
        /// <param name="island2">The second island.</param>
        public Bridge(Island island1, Island island2)
        {
            if (island1.Number < island2.Number)
            {
                Island1 = island1;
                Island2 = island2;
            }
            else
            {
                Island1 = island2;
                Island2 = island1;
            }
        }

        /// <summary>
        /// Gets the first island.
        /// </summary>
        public Island Island1 { get; }

        /// <summary>
        /// Gets the second island.
        /// </summary>
        public Island Island2 { get; }
    }
}
