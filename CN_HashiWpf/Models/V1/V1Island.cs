namespace CNHashiWpf.Models.V1
{
    /// <summary>
    /// Represents an island in the game.
    /// </summary>
    public class V1Island
    {
        /// <summary>
        /// The x-coordinate of the island on the grid.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The y-coordinate of the island on the grid.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The amount of bridges needed for the island.
        /// </summary>
        public int BridgesNeeded { get; set; }

        /// <summary>
        /// The amount of bridges connected to the island.
        /// </summary>
        public int BridgesConnected { get; set; }

        /// <summary>
        /// Creates a new instance of the Island class.
        /// </summary>
        /// <param name="x">The x-coordinate of the island on the grid.</param>
        /// <param name="y">The y-coordinate of the island on the grid.</param>
        /// <param name="bridgesNeeded">The amount of bridges needed for the island.</param>
        public V1Island(int x, int y, int bridgesNeeded)
        {
            X = x;
            Y = y;
            BridgesNeeded = bridgesNeeded;
            BridgesConnected = 0;
        }
    }
}
