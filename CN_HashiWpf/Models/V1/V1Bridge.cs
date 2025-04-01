namespace CNHashiWpf.Models.V1
{
    /// <summary>
    /// Represents a bridge between two islands.
    /// </summary>
    public class V1Bridge
    {
        /// <summary>
        /// The first island that is connected by the bridge.
        /// </summary>
        public V1Island Island1 { get; set; }

        /// <summary>
        /// The second island that is connected by the bridge.
        /// </summary>
        public V1Island Island2 { get; set; }

        /// <summary>
        /// The amount of bridges between the islands.
        /// </summary>
        public int Count { get; set; }

        public V1Bridge(V1Island island1, V1Island island2)
        {
            Island1 = island1;
            Island2 = island2;
            Count = 1;
        }
    }
}
