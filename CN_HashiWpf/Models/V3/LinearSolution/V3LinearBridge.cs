namespace CNHashiWpf.Models.V3.LinearSolution
{
    // Kante zwischen node1 und node2, der Knoten mit der höheren Nummer kommt an die erste Stelle
    public class V3LinearBridge
    {
        public V3LinearIsland Node1 { get; }
        public V3LinearIsland Node2 { get; }

        public V3LinearBridge(V3LinearIsland node1, V3LinearIsland node2)
        {
            if (node1.Number < node2.Number)
            {
                Node1 = node1;
                Node2 = node2;
            }
            else
            {
                Node1 = node2;
                Node2 = node1;
            }
        }
    }
}
