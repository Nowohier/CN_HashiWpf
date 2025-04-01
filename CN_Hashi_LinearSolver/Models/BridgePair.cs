namespace CNHashiLinearSolver.Models
{
    /// <summary>
    /// Represents a pair of bridges.
    /// </summary>
    public class BridgePair
    {
        public BridgePair(int bridge1Node1, int bridge1Node2, int bridge2Node1, int bridge2Node2)
        {
            Bridge1 = new int[2] { bridge1Node1, bridge1Node2 };
            Bridge2 = new int[2] { bridge2Node1, bridge2Node2 };
        }

        /// <summary>
        /// The first bridge.
        /// </summary>
        public int[] Bridge1 { get; }

        /// <summary>
        /// The second bridge.
        /// </summary>
        public int[] Bridge2 { get; }
    }
}
