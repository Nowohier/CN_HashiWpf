namespace CNHashiWpf.Models.V3.LinearSolution
{
    // Ein Paar aus Kanten
    public class V3LinearBridgePair
    {
        public int[] Edge1 { get; }
        public int[] Edge2 { get; }

        public V3LinearBridgePair(int edge1Node1, int edge1Node2, int edge2Node1, int edge2Node2)
        {
            Edge1 = new int[2] { edge1Node1, edge1Node2 };
            Edge2 = new int[2] { edge2Node1, edge2Node2 };
        }
    }
}
